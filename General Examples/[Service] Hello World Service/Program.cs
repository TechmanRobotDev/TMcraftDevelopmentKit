using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO.Ports;
using System.Reflection;
using System.IO;

using System.Runtime.InteropServices;
using System.Reflection.Metadata;

namespace TMcraftServiceTemplate
{
    class Program
    {
        const int port = 37001; //availale range for TMcraft Service: 37000~37099
        
        static async Task Main(string[] args)
        {
            //StartServer;
            try
            {
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new(ipAddress, port);
                // Create a TCP socket listener
                var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(ipEndPoint);
                listener.Listen(100); // Backlog queue size

                Console.WriteLine($"Server listening on port {port}");

                while (true)
                {
                    // Wait for a client connection
                    var clientSocket = await listener.AcceptAsync();
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] Client connected!");

                    // Handle communication with the client
                    await HandleClient(clientSocket);

                    // Close the client socket when communication is done
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }           
            
        }       
        
        static async Task HandleClient(Socket clientSocket)
        {
            while (clientSocket.Connected)
            {
                try
                {
                    // Receive message from client
                    var buffer = new byte[1024];
                    var received = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);

                    if (received == 0)
                    {
                        // Client disconnected
                        Console.WriteLine("Client disconnected!");
                        break;
                    }

                    // Decode received message
                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] Message Recieved: " + message);

                    // Process message and send response
                    string response;
                    if (message.Trim() == "Hello")
                    {
                        response = "World";
                    }
                    else if (message.Trim() == "Ciao")
                    {
                        response = "Ci vediamo!";
                    }
                    else
                    {
                        response = "InvalidCommand";
                    }

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await clientSocket.SendAsync(responseBytes, SocketFlags.None);
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] Response send: " + response.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while handling client: {ex.Message}");
                    break;
                }
            }
        }
    }
}

