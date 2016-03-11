using System;
using System.Text;

namespace RespServer.Protocol
{
    class RespProtocolReader
    {
        public static RespMarker ReadMarker(byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return new RespMarker(RespMarker.MarkerType.Error, -1);
            }

            switch (buffer[0])
            {
                case (byte)'*':
                    return new RespMarker(RespMarker.MarkerType.Array, int.Parse(Encoding.ASCII.GetString(buffer, 1, buffer.Length - 1)));
                case (byte)'$':
                    return new RespMarker(RespMarker.MarkerType.String, int.Parse(Encoding.ASCII.GetString(buffer, 1, buffer.Length - 1)));
            }

            return new RespMarker(RespMarker.MarkerType.Error, -1);
        }
    }
}
