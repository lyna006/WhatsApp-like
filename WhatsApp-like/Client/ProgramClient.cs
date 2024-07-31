using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || !int.TryParse(args[1], out int port))
            {
                Console.WriteLine("Usage: dotnet run <server-ip> <port>");
                return;
            }

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(IPAddress.Parse(args[0]), port);

            try
            {
                clientSocket.Connect(endPoint);
                Console.WriteLine("Connected to server.");
		Console.WriteLine("For private messages use the following format: /private <recipientId> <message>");

                Task.Run(() => ReceiveMessages(clientSocket));

                while (true)
                {
                    var message = Console.ReadLine();

                    if (message.StartsWith("/private"))
                    {
                        // /private <recipientId> <message>
                        var buffer = Encoding.UTF8.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                    else
                    {
                        var buffer = Encoding.UTF8.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                clientSocket.Close();
            }
        }

        private static void ReceiveMessages(Socket clientSocket)
        {
            var buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = clientSocket.Receive(buffer)) > 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
        }
    }
}
