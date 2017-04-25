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
            Console.WriteLine("*******************************************");
            Console.WriteLine("********** Dreadline Gate Server **********");
            Console.WriteLine("*******************************************");
            Console.WriteLine("\n");
            Console.WriteLine("Server will start in 10s");
            Thread.Sleep(10000);
            new Thread(new ThreadStart(Initializer)).Start();


            while(true)
            {
                for(int i = 0; i < functionlist.Count; i++)
                {
                    Console.WriteLine(functionlist[i](true));
                    functionlist.RemoveAt(i);
                }
            }
        }

        private static void Initializer()
        {
            functionlist.Add(delegate { return FUNCTIONS.Init(); });
            try
            {
                Console.WriteLine("Waiting For Clients...");
                while (true)
                {
                    if (server.isListing)
                    {
                        
                        server.clients.Add(new Client(server.socket.Accept()));
                    }
                }
            }
            catch
            {

            }
        }
    }
}
