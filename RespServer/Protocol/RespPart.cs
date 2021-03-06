﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mina.Core.Session;

namespace RespServer.Protocol
{
    public class RespPart
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

                case RespMarker.MarkerType.Integer:
                    return int.Parse(Encoding.ASCII.GetString(Body.ToArray()));

                case RespMarker.MarkerType.SimpleString:
                    return Body.TakeWhile((a)=>a!='\r' && a != '\n').ToArray();
            }

            return null;
        }

        public static RespPart Error(String str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            bool containsNl = data.Any((a) => a == '\n');
            if (containsNl)
            {
                return new RespPart(new RespMarker(RespMarker.MarkerType.String, data.Length), data);
            }
            return new RespPart(new RespMarker(RespMarker.MarkerType.Error, data.Length), data);
        }   

        public byte[] Serialize()
        {
            String ret = _marker.Serialize();
            if (_marker.Type == RespMarker.MarkerType.String || _marker.Type == RespMarker.MarkerType.Error)
            {
                ret += Encoding.ASCII.GetString(Body.ToArray()) + "\r\n";
            }
            return Encoding.ASCII.GetBytes(ret);
        }

        public static RespPart String(byte[] data)
        {
            return new RespPart(new RespMarker(RespMarker.MarkerType.String, data.Length), data);
        }

        public static RespPart String(string str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            return String(data);
        }

        public static RespPart Array(int n)
        {
            return new RespPart(new RespMarker(RespMarker.MarkerType.Array, n), new byte[]{});
        }

        public virtual void Write(IoSession socket)
        {
            socket.Write(this);
        }
    }
}