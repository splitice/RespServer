using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Transport.Socket;
using RespServer.Commands;
using RespServer.Mina;
using RespServer.Protocol;

namespace RespServer
{
    public class RespServerListener
    {
        private readonly CommandRegistry _commands;
        private IoAcceptor _server;
        private Dictionary<IoSession,RespConnection> _connections = new Dictionary<IoSession, RespConnection>();
        public bool Started;

        public RespServerListener(CommandRegistry commands)
        {
            _commands = commands;
            _server = new AsyncSocketAcceptor();
        }

        public void Start(IPEndPoint bind)
        {
            if (Started)
            {
                throw new Exception("Already Started");
            }

            _server.SessionOpened += (s,e) =>
            {
                RespConnection connection = new RespConnection(e.Session, _commands);
                _connections.Add(e.Session,connection);
                connection.OnDisconnect += (a, b) => _connections.Remove(e.Session);
            };
            _server.SessionClosed += (s, e) =>
            {
                _connections[e.Session].HandleDisconnect();
            };
            _server.MessageReceived += (s, e) =>
            {
                _connections[e.Session].HandleMessage(e.Message as RespPart);
            };


            _server.FilterChain.AddLast("codec", new ProtocolCodecFilter(new RespProtocolFactory()));

            _server.Bind(bind);

            Started = true;
        }
    }
}
