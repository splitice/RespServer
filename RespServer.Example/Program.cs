using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var registry = new RespCommandRegistry();
            RespServerListener server = new RespServerListener(registry);
            server.Start(new IPEndPoint(IPAddress.Any, 7777));
            while (server.Started)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
