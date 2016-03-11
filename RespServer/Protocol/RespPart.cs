using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace RespServer.Protocol
{
    internal class RespPart
    {
        private readonly RespMarker _marker;
        private readonly IEnumerable<byte> _body;

        public RespPart(RespMarker marker, IEnumerable<byte> body)
        {
            _marker = marker;
            _body = body;
        }

        public RespMarker Marker
        {
            get { return _marker; }
        }

        public IEnumerable<byte> Body
        {
            get { return _body; }
        }

        public object DeserializeScalar()
        {
            switch (_marker.Type)
            {
                case RespMarker.MarkerType.String:
                    return Body.ToArray();
            }

            return null;
        }
    }
}