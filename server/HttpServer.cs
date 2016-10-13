using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace server
{
    public class HttpServer : IDisposable
    {
        // Class private members
        private HttpServerState _state = HttpServerState.Stopped;
        private TcpListener _listener;
        private bool _disposed = false;
        private object _syncLock = new object();
        private Dictionary<HttpClient, bool> _clients = new Dictionary<HttpClient, bool>();
        private AutoResetEvent _clientsChangedEvent = new AutoResetEvent(false);

        // Fields/members used by Properties
        private int _port;
        private IPEndPoint _endPoint;
        private int _readBufferSize;
        private int _writeBufferSize;
        private string _serverBanner;
        private TimeSpan _readTimeout;
        private TimeSpan _writeTimeout;
        private TimeSpan _shutdownTimeout;

        #region Constructors

        public HttpServer()
        {
            Port = 8081;
            EndPoint = new IPEndPoint(IPAddress.Loopback, Port);
            ReadBufferSize = 4096;
            WriteBufferSize = 4096;
            ServerBanner = String.Format("PUCMM_HTTP/{0}", GetType().Assembly.GetName().Version);
            ReadTimeout = new TimeSpan(0, 1, 30);
            WriteTimeout = new TimeSpan(0, 1, 30);
            ShutdownTimeout = new TimeSpan(0, 0, 30);
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            _listener = new TcpListener(EndPoint);
            _state = HttpServerState.Starting;
            _listener.Start();
            EndPoint = (IPEndPoint)_listener.LocalEndpoint;
            _state = HttpServerState.Started;
        }

        public void Stop()
        {
            _state = HttpServerState.Stopping;
            _listener.Stop();
            _listener = null;
            _state = HttpServerState.Stopped;
        }

        public void Dispose()
        {
            _disposed = true;
        }

        #endregion

        #region Private Methods

        private void BeginAcceptTcpClient()
        { }

        private void AcceptTcpClientCallback(IAsyncResult iar)
        { }

        private void RegisterClient(HttpClient client)
        { }

        private void VerifyState(HttpServerState state)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (_state != state)
                throw new InvalidOperationException(String.Format("Expected server to be in the '{0}' state", state));
        }

        #endregion

        #region Properties

        public int Port
        {
            get { return _port; }
            private set { _port = value; }
        }

        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        public int ReadBufferSize
        {
            get { return _readBufferSize; }
            set { _readBufferSize = value; }
        }

        public int WriteBufferSize
        {
            get { return _writeBufferSize; }
            set { _writeBufferSize = value; }
        }

        public string ServerBanner
        {
            get { return _serverBanner; }
            set { _serverBanner = value; }
        }

        public TimeSpan ReadTimeout
        {
            get { return _readTimeout; }
            set { _readTimeout = value; }
        }

        public TimeSpan WriteTimeout
        {
            get { return _writeTimeout; }
            set { _writeTimeout = value; }
        }

        public TimeSpan ShutdownTimeout
        {
            get { return _shutdownTimeout; }
            set { _shutdownTimeout = value; }
        }

        #endregion

    }
}
