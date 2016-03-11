using System;

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
                    return new RespMarker(RespMarker.MarkerType.Array, BitConverter.ToInt32(buffer, 1));
                case (byte)'$':
                    return new RespMarker(RespMarker.MarkerType.String, BitConverter.ToInt32(buffer, 1));
            }

            return new RespMarker(RespMarker.MarkerType.Error, -1);
        }
    }
}
