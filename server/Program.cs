using System;
using System.IO;
using System.Diagnostics;

namespace server
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Initializing Server App... ");
            
            int portNum;
            string port = null;
            string path = null;

            // Getting args
            if (args.Length > 1)
            {
                if (args[0] == "--port") { port = args[1]; }
                if (args[0] == "--path") { path = args[1]; }
            }

            if (args.Length > 3)
            {
                if (args[2] == "--port") { port = args[3]; }
                if (args[2] == "--path") { path = args[3]; }
            }

            // Validating Port
            if (!int.TryParse(port, out portNum))
            {
                if (port != null)
                {
                    Console.WriteLine("ERROR: Port parameter provided is not a integer number.");
                    return 1;
                }
                portNum = 8081;
            }

            // Validating Path
            if (path != null)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("The path \"{0}\" does not exist.", path);
                    return 1;
                }
            }
            else
            {
                path = Directory.GetCurrentDirectory();
            }

            // Notifying App initialization
            Console.WriteLine("Server App initialization Complete!");
            Console.WriteLine($"Using PORT : {portNum}");
            Console.WriteLine($"Using PATH : {path}");
            Console.WriteLine();

            //--------------- Listen --------------//

            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //string cmd;

            Console.WriteLine($"Listening on port {portNum}...");

            HttpServer httpServer = new HttpServer(portNum);
            httpServer.Start();

            //while (true)
            //{
            //    cmd = Console.ReadLine();
            //    if (cmd.ToLower() == "uptime")
            //    {
            //        Console.WriteLine("Time since Server started: " + stopWatch.Elapsed);
            //    }
            //    else if (cmd.ToLower() == "break")
            //    { break; }
            //};

            httpServer.Stop();
            //stopWatch.Stop();

            return 0;
        }
    }
}
