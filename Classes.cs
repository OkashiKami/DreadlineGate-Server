using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Dreadline_Gate_Server
{
    public class FUNCTIONS
    {
        internal static string Title()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("*******************************************");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("**********");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Dreadline Gate Server ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("**********");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("*******************************************");
            Program.server = new Server();
            return string.Empty;
        }
        internal static string Init()
        {
            Console.WriteLine("Please Wait, Starting in 3s...");
            Thread.Sleep(3000);
            Program.server = new Server();
            Console.WriteLine("Please Wait, Creating server Socket...");
            Program.server.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread.Sleep(1000);
            Console.WriteLine("Please Wait, Creating IPEndpoint...");
            Program.server.endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2655);
            Thread.Sleep(500);
            Console.WriteLine("Please Wait, Starting up master server...");
            Thread.Sleep(2000);
            try
            {
                Program.server.socket.Bind(Program.server.endpoint);
            }
            catch
            {
                Console.Write("Connection: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(", Connecting to server Failed... (E:805:655)");
            }
            Thread.Sleep(500);
            if (Program.server.socket.IsBound)
            {
                Program.server.socket.Listen(100);
                Program.server.isListing = true;
                Console.Write("Connection: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("PASSED");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(", Connecting to server success");
                Thread.Sleep(300);
                Console.WriteLine("Please Wait, Getting enviorment ready for clients...");
                Thread.Sleep(2000);
                Console.WriteLine("Listening: {0}", Program.server.isListing);
                Thread.Sleep(500);
                Console.WriteLine("Waiting For Clients...");
            }
            return string.Empty;
        }

        internal static string ClientConnect()
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("+");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.WriteLine("Client connected");
            return string.Empty;
        }
        internal static string ClientDisconnect()
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("-");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.WriteLine("Client disconnected");
            return string.Empty;
        }
    }
    public class Server
    {
        internal List<Client> clients = new List<Client>();
        internal IPEndPoint endpoint;
        internal bool isListing;
        internal Socket socket;
    }
    public class Client
    {
        public Socket socket;
        public bool isConnected;
        public Client(Socket socket)
        {
            this.socket = socket;
            isConnected = true;
            new Thread(new ThreadStart(testSend)).Start();
            new Thread(new ThreadStart(Receiving)).Start();
        }

        public void Send(string message)
        {
            try
            {
                socket.Send(Encoder.Genarate("logininfo=INFO-" + message + "|"));
            }
            catch
            {
                try { socket.Shutdown(SocketShutdown.Both); } catch { }
                try { socket.Close(); } catch { }
                isConnected = false;
            }
        }
        private void Receiving()
        {
            while (isConnected)
            {
                try
                {
                    byte[] buff = new byte[socket.ReceiveBufferSize];
                    socket.Receive(buff, 0, buff.Length, SocketFlags.None);
                    string message = Encoder.Genarate(buff);
                    message = message.Split('|')[0];
                    if (!message.Equals(""))
                    {
                        //Console.WriteLine("Client Message: '" + message + "'");
                        Encoder.Decode(this, message);
                    }
                }
                catch
                {
                    try { socket.Shutdown(SocketShutdown.Both); } catch { }
                    try { socket.Close(); } catch { }
                    isConnected = false;
                }
            }
        }

        void testSend()
        {
            while (isConnected)
            {
                try
                {
                    socket.Send(Encoder.Genarate(""));
                }
                catch
                {
                    try { socket.Shutdown(SocketShutdown.Both); } catch { }
                    try { socket.Close(); } catch { }
                    isConnected = false;
                }
            }
        }
    }

    public static class Encoder
    {
        public static byte[] Genarate(string input) { return Encoding.UTF8.GetBytes(input); }
        public static string Genarate(byte[] input) { return Encoding.UTF8.GetString(input); }

        internal static void Decode(Client client, string ms)
        {
            string value = string.Empty;
            if (ms.StartsWith("logininfo"))
            {
                value = ms.Split('=')[1];
            }
            string[] values = value.Split(':');

            string user = string.Empty;
            string pass = string.Empty;
            string info = string.Empty;

            for (int i = 0; i < value.Length - 1; i++)
            {
                if (values[i].StartsWith("USER-"))
                {
                    user = values[i].Replace("USER-", "");
                    Console.WriteLine("From Client: USER = {0}", user);
                }
                else if (values[i].StartsWith("PASS-"))
                {
                    pass = values[i].Replace("PASS-", "");
                    Console.WriteLine("From Client: PASS = {0}", pass);
                }
                else if (values[i].StartsWith("INFO-"))
                {
                    info = values[i].Replace("INFO-", "");
                    Console.WriteLine("From Client: {0}", info);
                }
            }
            client.Send("Success Login");
        }
    }
}
