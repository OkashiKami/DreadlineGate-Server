using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dreadline_Gate_Server
{
    class FUNCTIONS
    {
        internal static string Init()
        {
            Console.WriteLine("Please Wait, Starting in 10s...");
            Thread.Sleep(10000);
            Console.WriteLine("Please Wait, Starting master server...");
            Thread.Sleep(3000);
            Program.server = new Server();
            Console.WriteLine("Please Wait, Creating Server Socket...");
            Program.server.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Thread.Sleep(3000);
            Console.WriteLine("Please Wait, Creating IPEndpoint...");
            Program.server.endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2655);
            Thread.Sleep(4000);
            Console.WriteLine("Please Wait, Binding...");
            Thread.Sleep(3000);
            try
            {
                Program.server.socket.Bind(Program.server.endpoint); ;
            }
            catch
            {
                Console.Write("Binding: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(", binding server Failed... (E:805:655)");
            }
            Thread.Sleep(5000);
            if (Program.server.socket.IsBound)
            {
                Console.Write("Binding: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("PASSED");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(", binding server was a success");
                Program.server.socket.Listen(100);
                Program.server.isListing = true;
            }
            return "Server Successfully Initialized";
        }
    }

    public class Server
    {
        internal Socket socket;
        internal IPEndPoint endpoint;
        internal bool isListing;

        internal List<Client> clients = new List<Client>();
    }

    public class Client
    {
        internal Socket socket;
        Thread SendThread;
        Thread ReceiveThread;
        public Client(Socket socket)
        {
            this.socket = socket;
            SendThread = new Thread(new ParameterizedThreadStart(send));
            ReceiveThread = new Thread(new ThreadStart(receive));
            Console.WriteLine("Client Connected");
            ReceiveThread.Start();
        }

        public void Send(string Message)
        {
            SendThread.Start(new object[] { this.socket, Message });
        }

        private void send(object obj)
        {
            try
            {
                Socket sock = (Socket)((object[])obj)[0];
                string mess = (string)((object[])obj)[1];

                byte[] buffer = Encoding.UTF8.GetBytes(mess);
                sock.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch
            {

            }
        }
        private void receive()
        {
            while(true)
            {
                try
                {
                    byte[] buffer = new byte[socket.ReceiveBufferSize];
                    socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    string incomming = Encoding.UTF8.GetString(buffer);
                    incomming = incomming.Replace(" ", "");
                    Console.WriteLine("From Client: {0}", incomming);
                }
                catch
                {

                }
            }
        }
    }
}
