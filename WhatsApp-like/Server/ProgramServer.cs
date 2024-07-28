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
        private static List<Client> _clients = new List<Client>();
        private static int _nextClientId = 1;

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
                Task.Run(() => HandleClient(clientSocket));
            }
        }

        private static void HandleClient(Socket clientSocket)
        {
            var clientId = _nextClientId++;
            var client = new Client(clientSocket, clientId);
            _clients.Add(client);

            var idMessage = Encoding.UTF8.GetBytes($"Your ID is: {clientId}");
            clientSocket.Send(idMessage);

            try
            {
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = clientSocket.Receive(buffer)) > 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from client {clientId}: {message}");
                    HandleMessage(message, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _clients.Remove(client);
                clientSocket.Close();
            }
        }

        private static void HandleMessage(string message, Client sender)
        {
            // Parse message for private communication
            if (message.StartsWith("/private"))
            {
                var parts = message.Split(new char[] { ' ' }, 3);
                if (parts.Length >= 3 && int.TryParse(parts[1], out int recipientId))
                {
                    var recipient = _clients.Find(c => c.Id == recipientId);
                    if (recipient != null)
                    {
                        var privateMessage = $"Private message from Client {sender.Id}: {parts[2]}";
                        var buffer = Encoding.UTF8.GetBytes(privateMessage);
                        recipient.Socket.Send(buffer);
                        return;
                    }
                }
                var errorMessage = Encoding.UTF8.GetBytes("Invalid private message format or recipient not found.");
                sender.Socket.Send(errorMessage);
            }
            else
            {
                BroadcastMessage(message, sender);
            }
        }

        private static void BroadcastMessage(string message, Client sender)
        {
            var buffer = Encoding.UTF8.GetBytes($"Client {sender.Id}: {message}");
            foreach (var client in _clients)
            {
                if (client != sender)
                {
                    client.Socket.Send(buffer);
                }
            }
        }
    }

    public class Client
    {
        public Socket Socket { get; set; }
        public int Id { get; set; }

        public Client(Socket socket, int id)
        {
            Socket = socket;
            Id = id;
        }
    }
}
