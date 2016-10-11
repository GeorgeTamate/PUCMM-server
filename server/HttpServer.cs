using System.Net;
using System.Net.Sockets;

namespace server
{
    public class HttpServer
    {
        HttpServerState State;
        TcpListener tcpListener;
        IPEndPoint endPoint;

        public HttpServer()
        {
            endPoint = new IPEndPoint(IPAddress.Loopback, 8081);
            tcpListener = new TcpListener(endPoint);
        }

        public void Start()
        {
            tcpListener.Start();
        }

        public void Stop()
        {
            tcpListener.Stop();
        }

        
    }
}
