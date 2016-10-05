using System;
using System.Threading;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Initializing Server... ");

            string port = null;
            string path = null;
            if (args.Length > 1) { port = args[0]; }
            if (args.Length > 1) { path = args[1]; }

            if (args.Length > 1)
            {
                if (args[0] == "--port"){ port = args[1]; }
                if (args[0] == "--path"){ path = args[1]; }
            }

            if (args.Length > 3)
            {
                if (args[2] == "--port"){ port = args[3]; }
                if (args[2] == "--path"){ path = args[3]; }
            }

            Server server = new Server(port, path);

            Console.WriteLine("Starting Thread... ");

            Thread thread = new Thread(new ThreadStart(server.listen));
            thread.Start();
        }
    }
}
