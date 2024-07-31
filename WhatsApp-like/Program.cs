using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProgramaCentral
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configura los argumentos para el cliente y el servidor
            string serverPort = "5000"; // Puerto del servidor
            string clientIp = "127.0.0.1"; // IP del servidor
            string clientPort = serverPort; // Puerto del cliente

            // Ruta a los ejecutables de cliente y servidor
            string serverPath = @"..\ChatServer\bin\Debug\net8.0\ChatServer.dll";
            string clientPath = @"..\ChatClient\bin\Debug\net8.0\ChatClient.dll";

            // Inicia el servidor
            var serverProcess = StartProcess(serverPath, serverPort);
            Console.WriteLine("Servidor iniciado.");

            // Da tiempo al servidor para iniciar
            await Task.Delay(2000);

            // Inicia múltiples clientes
            var clientProcesses = new[]
            {
                StartProcess(clientPath, $"{clientIp} {clientPort}"),
                StartProcess(clientPath, $"{clientIp} {clientPort}"),
                StartProcess(clientPath, $"{clientIp} {clientPort}")
            };

            Console.WriteLine("Clientes iniciados.");

            // Espera a que los procesos terminen
            await Task.WhenAll(serverProcess, Task.WhenAll(clientProcesses));
        }

        private static Task StartProcess(string path, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{path} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            // Opcional: Muestra la salida del proceso
            Task.Run(() =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    Console.WriteLine(process.StandardOutput.ReadLine());
                }
            });

            Task.Run(() =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    Console.WriteLine(process.StandardError.ReadLine());
                }
            });

            return Task.Run(() => process.WaitForExit());
        }
    }
}
