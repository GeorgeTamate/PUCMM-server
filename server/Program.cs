using System;
using System.Threading;

namespace server
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Initializing Server... ");

            string port = null;
            string path = null;
            

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


            Server server;

            try
            {
                server = new Server(port, path);
            }
            catch
            {
                //Console.ReadLine();
                return 1;
            }
            

            Console.WriteLine("Starting Thread... ");

            Thread thread = new Thread(new ThreadStart(server.listen));
            thread.Start();

            return 0;
        }
    }
}
