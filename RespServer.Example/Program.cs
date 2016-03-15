using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RespServer.Commands;
using RespServer.Protocol;

namespace RespServer.Example
{
    class Program
    {
        class HelloCommand : IRespCommand
        {
            public IEnumerable<RespPart> Execute(RespConnection connection)
            {
                return new[] { RespPart.String("Hello World") };
            }
        }
        static void Main(string[] args)
        {
            var registry = new RespCommandRegistry();
            registry.RegisterCommand("HELLO", (a)=>new HelloCommand());
            RespServerListener server = new RespServerListener(registry);
            server.Start(new IPEndPoint(IPAddress.Any, 7777));
            while (server.Started)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
