using Server.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        readonly static ServerHandler ServerHandler;
        static Program()
        {
            ServerHandler = new ServerHandler();
        }
        static void Main(string[] args)
        {
            beginn:
            var cmd = Console.ReadLine();
            if (cmd == "exit")
            {
                ServerHandler.Close();
            }
            else if(cmd == "clear")
            {
                Console.Clear();
                goto beginn;
            }
            else
            {
                Console.WriteLine("Invalid Command");
                goto beginn;
            }

        }
    }
}
