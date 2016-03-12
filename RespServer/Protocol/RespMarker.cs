using System;

namespace RespServer.Protocol
{
    public struct RespMarker
    {
        public enum MarkerType
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

        public bool ReadLine
        {
            get
            {
                if (Type == MarkerType.Integer || Type == MarkerType.Error)
                {
                    return true;
                }
                return false;
            }
        }

        public RespMarker(MarkerType type, int length)
        {
            Type = type;
            Length = length;
        }

        public string Serialize()
        {
            switch (Type)
            {
                case MarkerType.Array:
                    return String.Format("*{0}\r\n", Length);
                case MarkerType.String:
                    return String.Format("${0}\r\n", Length);
                case MarkerType.Integer:
                    return ":";
                case MarkerType.Boolean:
                    return "+";
                case MarkerType.Error:
                    return "-";
                case MarkerType.EOF:
                    return "-1";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
