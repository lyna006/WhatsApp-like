using System;
using System.Diagnostics;

namespace ChatMenu
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start the server
            var serverPort = "5000";
            StartServer(serverPort);

            // Show menu
            while (true)
            {
                Console.WriteLine("1. Add Client");
                Console.WriteLine("2. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();

                if (input == "1")
                {
                    Console.Write("Enter server IP (default 127.0.0.1): ");
                    var serverIp = Console.ReadLine();
                    if (string.IsNullOrEmpty(serverIp))
                    {
                        serverIp = "127.0.0.1";
                    }

                    OpenClient(serverIp, serverPort);
                }
                else if (input == "2")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option. Try again.");
                }
            }
        }

        static void StartServer(string port)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project ../Server {port}",
                UseShellExecute = true
            };

            Process.Start(processInfo);
            Console.WriteLine($"Server started on port {port}");
        }

        static void OpenClient(string serverIp, string serverPort)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project ../Client {serverIp} {serverPort}",
                UseShellExecute = true
            };

            Process.Start(processInfo);
        }
    }
}
