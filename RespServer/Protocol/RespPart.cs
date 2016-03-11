using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

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
                case RespMarker.MarkerType.Error:
                    return Body.ToArray();

                case RespMarker.MarkerType.String:
                    return Body.ToArray();
            }

            return null;
        }

        public static RespPart Error(String str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            return new RespPart(new RespMarker(RespMarker.MarkerType.Error, data.Length), data);
        }

        public byte[] Serialize()
        {
            String ret = _marker.Serialize();
            if (_marker.ReadLength != 0)
            {
                ret += Encoding.ASCII.GetString(Body.ToArray());
            }
            return Encoding.ASCII.GetBytes(ret);
        }
    }
}