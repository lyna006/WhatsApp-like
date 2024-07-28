using System;
using System.Diagnostics;

namespace ChatMenu
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Add Client");
                Console.WriteLine("2. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();

                if (input == "1")
                {
                    Console.Write("Enter server IP: ");
                    var serverIp = Console.ReadLine();

                    Console.Write("Enter server port: ");
                    var serverPort = Console.ReadLine();

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

        static void OpenClient(string serverIp, string serverPort)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project ../ChatClient {serverIp} {serverPort}",
                UseShellExecute = true
            };

            Process.Start(processInfo);
        }
    }
}
