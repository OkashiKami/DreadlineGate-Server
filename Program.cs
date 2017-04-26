using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Dreadline_Gate_Server
{
    class Program
    {
        public static Server server;
        public static List<Func<bool, string>> functionlist = new List<Func<bool, string>>();
        static void Main(string[] args)
        {
            functionlist.Add(delegate { return FUNCTIONS.Title(); });
            new Thread(new ThreadStart(Initializer)).Start();


            while(true)
            {
                try
                {
                    for (int i = 0; i < functionlist.Count; i++)
                    {
                        Console.WriteLine(functionlist[i](true));
                        functionlist.RemoveAt(i);
                    }
                }
                catch { }

                try
                {
                    for (int i = 0; i < server.clients.Count; i++)
                    {
                        if (!server.clients[i].isConnected)
                        {
                            try { server.clients[i].socket.Shutdown(SocketShutdown.Both); } catch { }
                            try { server.clients[i].socket.Close(); } catch { }
                            functionlist.Add(delegate { return FUNCTIONS.ClientDisconnect(); });
                            server.clients.RemoveAt(i);
                        }
                    }
                }
                catch { }
            }
        }

        private static void Initializer()
        {
            functionlist.Add(delegate { return FUNCTIONS.Init(); });
           
            while (true)
            {
                try
                {
                    if (server.isListing)
                    {
                        
                        server.clients.Add(new Client(server.socket.Accept()));
                        functionlist.Add(delegate { return FUNCTIONS.ClientConnect(); });
                        
                    }
                }
                catch
                {

                }
            }
            
        }
    }
}
