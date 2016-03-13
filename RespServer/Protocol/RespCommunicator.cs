using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespServer.Protocol
{
    /*class RespCommunicator
    {
        private RespParser _parser = new RespParser();
        private readonly IObservable<byte> _observable;
        public event EventHandler<RespEvent> MessageArrived;

        public RespCommunicator(IObservable<byte> observable)
        {
            _observable = observable;
        }

        private void HandleCommand(RespPart message)
        {
            List<object> handled;
            try
            {
                handled = _parser.MessageHandle(message);
            }
            catch (Exception ex)
            {
                if (MessageArrived != null)
                {
                    MessageArrived.Invoke(this, new RespEvent {Exception = ex});
                }
                return;
            }
            if (handled != null && MessageArrived != null)
            {
                MessageArrived.Invoke(this, new RespEvent{Arguments = handled});
            }
        }

        public void Start()
        {
            IObservable<RespPart> messages = from header in _observable.TakeWhileInclusive((a) => a != '\n').ToArray()
                                             let marker = RespProtocolReader.ReadMarker(header.ToArray())
                                             let body = marker.ReadLine ? _observable.TakeWhileInclusive((a) => a != '\n') : _observable.Take(marker.ReadLength)
                                             select new RespPart(marker, body.ToEnumerable());

            messages.Subscribe(HandleCommand);
            messages.Catch<RespPart, Exception>(tx => Observable.Empty<RespPart>());
        }
    }*/
}
