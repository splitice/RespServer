using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RespServer.Commands;

namespace RespServer.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var registry = new CommandRegistry();
            RespServerListener server = new RespServerListener(7777, registry);
            server.Start();
            while (server.Started)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
