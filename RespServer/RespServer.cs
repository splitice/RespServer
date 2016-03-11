using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveSockets;

namespace RespServer
{
    class RespServer
    {
        private ReactiveListener _server;
        private HashSet<RespConnection> _connections = new HashSet<RespConnection>();
        public bool Started;

        public RespServer(int port)
        {
            _server = new ReactiveListener(1055);
        }
        public void Start()
        {
            if (Started)
            {
                throw new Exception("Already Started");
            }

            _server.Connections.Subscribe(socket =>
            {
                RespConnection connection = new RespConnection(socket);
                _connections.Add(connection);
                connection.OnDisconnect += (a, b) => _connections.Remove(a as RespConnection);
            });


            _server.Start();

            Started = true;
        }
    }
}
