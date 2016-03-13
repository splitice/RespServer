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
                    return new RespMarker(RespMarker.MarkerType.Array, IntMarkerParse(buffer));
                case (byte)'$':
                    return new RespMarker(RespMarker.MarkerType.String, IntMarkerParse(buffer));
            }

            return new RespMarker(RespMarker.MarkerType.Error, -1);
        }

        private static int IntMarkerParse(byte[] buffer)
        {
            int length = buffer.Length - 1;
            if (buffer[buffer.Length - 1] == '\n')
            {
                length--;
                if (buffer[buffer.Length - 2] == '\r')
                {
                    length--;
                }
            }
            return int.Parse(Encoding.ASCII.GetString(buffer, 1, length));
        }
    }
}
