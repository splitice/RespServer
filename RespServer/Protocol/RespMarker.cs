using System;

namespace RespServer.Protocol
{
    struct RespMarker
    {
        internal enum MarkerType
        {
            Array, String, Integer, Boolean, Error, EOF
        }

        public MarkerType Type;
        public int Length;

        public int ReadLength
        {
            get
            {
                if (Type == MarkerType.String)
                {
                    return Length;
                }
                return 0;
            }
        }

        public RespMarker(MarkerType type, int length)
        {
            Type = type;
            Length = length;
        }
    }
}
