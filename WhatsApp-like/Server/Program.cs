using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        private static List<Socket> _clients = new List<Socket>();

        static void Main(string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int port))
            {
                Console.WriteLine("Usage: dotnet run <port>");
                return;
            }

            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(10);

            Console.WriteLine($"Server listening on port {port}");

            while (true)
            {
                var clientSocket = serverSocket.Accept();
                _clients.Add(clientSocket);
                Task.Run(() => HandleClient(clientSocket));
            }
        }

        private static void HandleClient(Socket clientSocket)
        {
            try
            {
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = clientSocket.Receive(buffer)) > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {message}");
                    BroadcastMessage(message, clientSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _clients.Remove(clientSocket);
                clientSocket.Close();
            }
        }

        private static void BroadcastMessage(string message, Socket sender)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients)
            {
                if (client != sender)
                {
                    client.Send(buffer);
                }
            }
        }
    }
}
