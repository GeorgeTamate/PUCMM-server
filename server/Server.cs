using System;
using System.IO;

namespace server
{
    class Server
    {
        int port;
        string rootpath;
        const int defaultPortNumber = 80;
        const string backslash = @"\";

        public Server()
        {
            port = defaultPortNumber;
            rootpath = Directory.GetCurrentDirectory();
            notifyInitReport();
        }

        public Server(int port, string path)
        {
            if (isPortAvailable(port))
            {
                this.port = port;
            }
            else
            {
                Console.WriteLine("ERROR: Port Not Available.");
                Console.WriteLine($"Default Port Number {defaultPortNumber} will be used instead.");
                Console.WriteLine();
                this.port = defaultPortNumber;
            }
            assignRootPath(path);
            notifyInitReport();
        }

        public Server(string port, string path)
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
                    Console.WriteLine("ERROR: Port Not Available.");
                    Console.WriteLine($"Default Port Number {defaultPortNumber} will be used instead.");
                    Console.WriteLine();
                    this.port = defaultPortNumber;
                }
            }
            else
            {
                if (port != null)
                {
                    Console.WriteLine($"ERROR: Port parameter provided is not a number.");
                    Console.WriteLine($"Default Port Number {defaultPortNumber} will be used instead.");
                    Console.WriteLine();
                }
                this.port = defaultPortNumber;
            }
            assignRootPath(path);
            notifyInitReport();
        }

        private bool isPortAvailable(int port)
        {
            
            return true;
        }

        private void assignRootPath(string path)
        {
            if (path != null)
            {
                rootpath = path;
            }
            else
            {
                rootpath = Directory.GetCurrentDirectory();
            }
        }

        public void notifyInitReport()
        {
            Console.WriteLine("Server initialization Complete!");
            Console.WriteLine($"Using PORT : {port}");
            Console.WriteLine($"Using PATH : {rootpath}");
            Console.WriteLine();
        }

        public void listen()
        {
            
            Console.WriteLine($"Listening on port {port}...");
            Console.WriteLine();

            // Loop for listening clients

            //while (true)
            //{
            //    // Accepting connection with new client
            //    client = server.AcceptTcpClient();
            //    // Handling request from client in a separate thread
            //    //Thread thread = new Thread(new ThreadStart());
            //    //thread.Start();
            //    //Thread.Sleep(100);
            //}
        }
    }
}
