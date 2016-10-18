using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace server
{
    public class HttpClient : IDisposable
    {
        // Class private members
        private HttpClientState _state = HttpClientState.Closed;
        private bool _disposed = false;
        private static readonly Regex PrologRegex = new Regex("^([A-Z]+) ([^ ]+) (HTTP/[^ ]+)$", RegexOptions.Compiled);

        // Fields/members used by Properties
        private TcpClient _tcpClient;
        private HttpServer _server;


        public HttpClient(HttpServer httpServer, TcpClient tcpClient)
        {
            if (httpServer == null)
            { throw new ArgumentNullException("HttpServer argument provided is null."); }
            Server = httpServer;

            if (tcpClient == null)
            { throw new ArgumentNullException("TcpClient argument provided is null."); }
            TcpClient = tcpClient;
        }


        public void BeginRequest()
        {
            Console.WriteLine("HttpClient.BeginRequest() method has been called.");
        }

        public void Dispose()
        {
            _disposed = true;
        }


        #region Properties

        public HttpServer Server
        {
            get { return _server; }
            private set { _server = value; }
        }

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            private set { _tcpClient = value; }
        }

        #endregion
    }
}
