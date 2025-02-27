using System;
using System.IO;
using System.IO.Pipes;
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
                        using (var pipeServer = new NamedPipeServerStream("DeathCounterPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                        {
                            Console.WriteLine("Named Pipe Server: Esperando conexión...");
                            pipeServer.WaitForConnection();

                            if (!running)
                                return; // Salir si el servidor ha sido detenido

                            Console.WriteLine("Cliente conectado.");

                            using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                            using (var reader = new StreamReader(pipeServer))
                            {
                                string command;
                                while (running && pipeServer.IsConnected && (command = reader.ReadLine()) != null)
                                {
                                    if (command == "get_deaths")
                                    {
                                        int deaths = Main.GetTotalPlayerDeaths();
                                        writer.WriteLine(deaths);
                                        Console.WriteLine($"Enviado: {deaths}");
                                    }
                                }
                            }

                            Console.WriteLine("Cliente desconectado, esperando nuevo cliente...");
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Named Pipe Server: Conexión cerrada inesperadamente. ({ex.Message})");
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
            using (NamedPipeClientStream client = new NamedPipeClientStream(".", "DeathCounterPipe", PipeDirection.Out))
            {
                try
                {
                    client.Connect(1000);
                }
                catch { }
            }
        }
    }
}
