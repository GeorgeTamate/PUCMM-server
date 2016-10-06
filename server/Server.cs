using System;
using System.IO;
using System.Net.NetworkInformation;

namespace server
{
    class Server
    {
        int port;
        string rootpath;
        const int defaultPortNumber = 80;
        const string backslash = @"\";

        public Server()
        { }
        

        public bool serverInit(string port, string path)
        {
            int portNum;
            if (int.TryParse(port, out portNum))
            {
                if (isPortAvailable(portNum))
                {
                    this.port = portNum;
                }
                else
                {
                    Console.WriteLine($"ERROR: Port {port} Not Available.");
                    return false;
                }
            }
            else
            {
                if (port != null)
                {
                    Console.WriteLine("ERROR: Port parameter provided is not a integer number.");
                    return false;
                }
                this.port = defaultPortNumber;
            }
            bool errorfree = true;
            errorfree = assignRootPath(path);
            if (!errorfree) { return false; }
            notifyInitReport();
            return true;
        }
        

        private bool isPortAvailable(int port)
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

        private bool assignRootPath(string path)
        {
            if (path != null)
            {
                if (Directory.Exists(path))
                {
                    rootpath = path;
                }
                else
                {
                    Console.WriteLine($"The path \"{path}\" does not exist.");
                    return false;
                }
            }
            else
            {
                rootpath = Directory.GetCurrentDirectory();
            }
            return true;
        }

        public void notifyInitReport()
        {
            Console.WriteLine("Server initialization Complete!");
            Console.WriteLine($"Using PORT : {port}");
            Console.WriteLine($"Using PATH : {rootpath}");
            Console.WriteLine();
        }

        public bool listen()
        {
            if (!isPortAvailable(port))
            {
                Console.WriteLine($"ERROR: Port {port} Not Available.");
                return false;
            }
            Console.WriteLine($"Listening on port {port}...");
            while (true){};
            return true;
        }
    }
}
