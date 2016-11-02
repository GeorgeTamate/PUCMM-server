﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace server
{
    public class HttpServer : IDisposable
    {
        // Class private members
        private TcpListener _listener;
        private bool _disposed = false;
        private object _syncLock = new object();
        private Dictionary<HttpClient, bool> _clients = new Dictionary<HttpClient, bool>();
        private AutoResetEvent _clientsChangedEvent = new AutoResetEvent(false);

        // Members used by Properties
        private HttpServerState _state;

        #region Constructors

        public HttpServer(int port)
        {
            if (IsPortAvailable(port))
            { Port = port; }
            else
            {
                Console.WriteLine("ERROR: Port {0} Not Available.", port);
                Port = 8081;  //Default port number
                Console.WriteLine("Using Port {0} instead.", Port);
            } 

            EndPoint = new IPEndPoint(IPAddress.Loopback, Port);
            State = HttpServerState.Stopped;
            ReadBufferSize = 4096;
            WriteBufferSize = 4096;
            ServerBanner = String.Format("PUCMM_HTTP/{0}", GetType().Assembly.GetName().Version);
            ReadTimeout = TimeSpan.FromSeconds(90);
            WriteTimeout = TimeSpan.FromSeconds(90);
            ShutdownTimeout = TimeSpan.FromSeconds(30);
        }

        #endregion

        #region Public and Internal Methods

        public void Start()
        {
            VerifyState(HttpServerState.Stopped);
            TimeoutManager = new HttpTimeoutManager(this);
            _listener = new TcpListener(EndPoint);
            ServerUtility = new HttpServerUtility();
            State = HttpServerState.Starting;
            try
            {
                _listener.Start();
                EndPoint = _listener.LocalEndpoint as IPEndPoint;
                State = HttpServerState.Started;
                BeginAcceptTcpClient();
            }
            catch
            {
                Console.WriteLine("The Server failed to start.");
                State = HttpServerState.Stopped;
                throw new PHttpException("Failed to start HTTP server.");
            }
        }

        public void Stop()
        {
            VerifyState(HttpServerState.Started);
            State = HttpServerState.Stopping;
            try
            {
                _listener.Stop();
            }
            catch
            {
                Console.WriteLine("The Server failed to stop.");
                throw new PHttpException("Failed to stop HTTP server.");
            }
            finally
            {
                StopClients();
                _listener = null;
                State = HttpServerState.Stopped;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_state == HttpServerState.Started)
                    Stop();
                if (_clientsChangedEvent != null)
                {
                    ((IDisposable)_clientsChangedEvent).Dispose();
                    _clientsChangedEvent = null;
                }
                _disposed = true;
            }

            if (TimeoutManager != null)
            {
                TimeoutManager.Dispose();
                TimeoutManager = null;
            }
        }

        internal void RaiseRequest(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            OnRequestReceived(new HttpRequestEventArgs(context));
        }

        internal bool RaiseUnhandledException(HttpContext context, Exception exception)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var e = new HttpExceptionEventArgs(context, exception);
            OnUnhandledException(e);
            return e.Handled;
        }

        internal void UnregisterClient(HttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("Client argument in HttpServer.UnregisterClient() method is null");
            }

            lock (_syncLock)
            {
                Debug.Assert(_clients.ContainsKey(client));
                _clients.Remove(client);
                _clientsChangedEvent.Set();
            }
        }



        #endregion

        #region Private Methods

        private void BeginAcceptTcpClient()
        {
            var listener = _listener;
            if(listener == null)
            {
                throw new NullReferenceException("Local listener from HttpServer.BeginAcceptTcpClient() is null.");
            }
            listener.BeginAcceptTcpClient(AcceptTcpClientCallback, listener);
        }

        private void AcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            try
            {
                var listener = _listener;
                if (listener == null) { return; }
                var tcpClient = listener.EndAcceptTcpClient(asyncResult);
                if (State == HttpServerState.Stopped) { tcpClient.Close(); }
                var client = new HttpClient(this, tcpClient);
                RegisterClient(client);
                client.BeginRequest();
                ////////listener.BeginAcceptTcpClient(AcceptTcpClientCallback, listener);
                BeginAcceptTcpClient();
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void RegisterClient(HttpClient client)
        {
            if (client == null)
            { throw new ArgumentNullException("HttpClient argument provided is null."); }
            lock (_syncLock)
            {
                _clients.Add(client, false);
                _clientsChangedEvent.Set();
            }
        }

        private void VerifyState(HttpServerState state)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (_state != state)
                throw new InvalidOperationException(String.Format("Expected server to be in the '{0}' state", state));
        }

        private bool IsPortAvailable(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
        }

        private void StopClients()
        {
            var shutdownStarted = DateTime.Now;
            bool forceShutdown = false;
            // Clients that are waiting for new requests are closed.

            List<HttpClient> clients;
            lock (_syncLock)
            {
                clients = new List<HttpClient>(_clients.Keys);
            }

            foreach (var client in clients)
            {
                client.RequestClose();
            }

            // First give all clients a chance to complete their running requests.
            while (true)
            {
                lock (_syncLock)
                {
                    if (_clients.Count == 0)
                        break;
                }

                var shutdownRunning = DateTime.Now - shutdownStarted;

                if (shutdownRunning >= ShutdownTimeout)
                {
                    forceShutdown = true;
                    break;
                }
                _clientsChangedEvent.WaitOne(ShutdownTimeout - shutdownRunning);
            }

            if (!forceShutdown)
                return;

            // If there are still clients running after the timeout, their
            // connections will be forcibly closed.
            lock (_syncLock)
            {
                clients = new List<HttpClient>(_clients.Keys);
            }

            foreach (var client in clients)
            {
                client.ForceClose();
            }

            // Wait for the registered clients to be cleared.
            while (true)
            {
                lock (_syncLock)
                {
                    if (_clients.Count == 0)
                        break;
                }
                _clientsChangedEvent.WaitOne();
            }
        }

        #endregion

        #region Protected Methods and Events

        public event EventHandler StateChanged;
        protected virtual void OnChangedState(EventArgs args)
        {
            var ev = StateChanged;
            if (ev != null)
            {
                ev(this, args);
            }
        }

        public event HttpRequestEventHandler RequestReceived;
        protected virtual void OnRequestReceived(HttpRequestEventArgs args)
        {
            var ev = RequestReceived;
            if (ev != null)
            {
                ev(this, args);
            }
        }

        public event HttpExceptionEventHandler UnhandledException;
        protected virtual void OnUnhandledException(HttpExceptionEventArgs args)
        {
            var ev = UnhandledException;
            if (ev != null)
            {
                ev(this, args);
            }
        }

        #endregion

        #region Properties

        public HttpServerState State
        {
            get { return _state; }
            private set
            {
                _state = value;
                OnChangedState(EventArgs.Empty);
            }
        }

        public int Port { get; private set; }

        public IPEndPoint EndPoint { get; set; }

        public int ReadBufferSize { get; set; }

        public int WriteBufferSize { get; set; }

        public string ServerBanner { get; set; }

        public TimeSpan ReadTimeout { get; set; }

        public TimeSpan WriteTimeout { get; set; }

        public TimeSpan ShutdownTimeout { get; set; }

        internal HttpServerUtility ServerUtility { get; private set; }

        internal HttpTimeoutManager TimeoutManager { get; private set; }


        #endregion

    }
}
