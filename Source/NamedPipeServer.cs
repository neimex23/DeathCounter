using Celeste.Mod.DeathCounter;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.DeathCounter
{
    public static class NamedPipeServer
    {
        private static bool running = true;

        public static void StartServer()
        {
            Task.Run(() =>
            {
                try
                {
                    while (running)
                    {
                        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("DeathCounterPipe", PipeDirection.InOut, 1))
                        {
                            Console.WriteLine("Named Pipe Server: Esperando conexión...");
                            pipeServer.WaitForConnection();
                            Console.WriteLine("Cliente conectado.");

                            using (StreamWriter writer = new StreamWriter(pipeServer) { AutoFlush = true })
                            using (StreamReader reader = new StreamReader(pipeServer))
                            {
                                string command;
                                while ((command = reader.ReadLine()) != null && running)
                                {
                                    if (command == "get_deaths")
                                    {
                                        int deaths = Main.GetTotalPlayerDeaths();
                                        writer.WriteLine(deaths);
                                        Console.WriteLine($"Enviado: {deaths}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Named Pipe Server Error: {ex.Message}");
                }
            });
        }

        public static void StopServer()
        {
            running = false;
        }
    }
}
