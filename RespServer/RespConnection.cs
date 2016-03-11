using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveSockets;
using RespServer.Protocol;

namespace RespServer
{
    class RespConnection
    {
        private ReactiveSocket _socket;
        public event EventHandler<EventArgs> OnDisconnect; 
        private RespParser _parser = new RespParser();

        public RespConnection(ReactiveSocket socket)
        {
            _socket = socket;

            IObservable<RespPart> messages = from header in socket.Receiver.TakeWhile((a) => a != '\n').ToArray()
                                             let marker = RespProtocolReader.ReadMarker(header.ToArray())
                                             let body = socket.Receiver.Take(marker.ReadLength)
                                             select new RespPart(marker, body.ToEnumerable());

            // Echo the incoming message with the same format.
            messages.Subscribe(message =>
            {
                var response = _parser.MessageHandle(message);

                if (response != null && response.Count != 0)
                {
                    
                }
                /*var body = _encoding.GetBytes(message);
                var header = BitConverter.GetBytes(body.Length);
                var payload = header.Concat(body).ToArray();

                socket.SendAsync(payload).Wait();*/
            });

            socket.Disconnected += (a,e) => { if(OnDisconnect != null) OnDisconnect(this, e); };
        }
    }
}
