using System;
using System.Net;
using System.Net.Sockets;

namespace server
{
    public class HttpServer : IDisposable
    {
        // Class private members
        private HttpServerState _state = HttpServerState.Stopped;
        private TcpListener _listener;
        private bool _disposed = false;

        // Fields/members used by Properties
        private int _port;
        private IPEndPoint _endPoint;


        public HttpServer()
        {
            Port = 8081;
            EndPoint = new IPEndPoint(IPAddress.Loopback, Port);
        }


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

        #endregion

    }
}
