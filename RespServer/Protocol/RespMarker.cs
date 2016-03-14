using System;

namespace RespServer.Protocol
{
    public struct RespMarker
    {
        public enum MarkerType
        {
            Array, String, Integer, SimpleString, Error, Null
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
                if (Type == MarkerType.Integer || Type == MarkerType.Error || Type == MarkerType.SimpleString)
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
                case MarkerType.SimpleString:
                    return "+";
                case MarkerType.Error:
                    return "-";
                case MarkerType.Null:
                    return "-1";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static RespMarker ReadMarker(byte typeChar, byte[] str)
        {
            switch (typeChar)
            {
                case (byte)'*':
                    return new RespMarker(RespMarker.MarkerType.Array, IntParse(str));
                case (byte)'$':
                    var len = IntParse(str);
                    if (len == -1)
                    {
                        return new RespMarker(MarkerType.Null, 0);
                    }
                    return new RespMarker(RespMarker.MarkerType.String, len);
                case (byte)':':
                    return new RespMarker(RespMarker.MarkerType.SimpleString, 0);
                case (byte)'+':
                    return new RespMarker(RespMarker.MarkerType.SimpleString, 0);
            }

            return new RespMarker(RespMarker.MarkerType.Error, -1);
        }

        private static int IntParse(byte[] str)
        {
            bool neg = false;
            int sum = 0;
            for (int index = 0; index < str.Length; index++)
            {
                var s = str[index];
                if (s == '\r' || s == '\n')
                {
                    break;
                }
                if (index == 0 && s == '-')
                {
                    neg = true;
                    continue;
                }
                unchecked
                {
                    s -= (byte)'0';
                }
                if (s > 9)
                {
                    throw new Exception("Invalid Integer");
                }
                sum = (sum*10) + s;
            }

            return neg ? -sum : sum;
        }
    }
}
