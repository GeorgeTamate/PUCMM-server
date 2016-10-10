using System;

namespace server
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Initializing Server... ");

            bool errorfree;
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

            Server server = new Server();
            errorfree = server.serverInit(port, path);
            if (!errorfree) { return 1; }
            
            errorfree = server.listen();
            if (!errorfree) { return 1; }

            return 0;
        }
    }
}
