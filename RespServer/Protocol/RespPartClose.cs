using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mina.Core.Session;

namespace RespServer.Protocol
{
    public class RespPartClose: RespPart
    {
        public RespPartClose(RespMarker marker, IEnumerable<byte> body) : base(marker, body)
        {
        }

        public override void Write(IoSession socket)
        {
            base.Write(socket);

            socket.Close(false);
        }


        public new static RespPart String(byte[] data)
        {
            return new RespPartClose(new RespMarker(RespMarker.MarkerType.String, data.Length), data);
        }

        public new static RespPart String(string str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            return String(data);
        }

        public new static RespPart Array(int n)
        {
            return new RespPartClose(new RespMarker(RespMarker.MarkerType.Array, n), new byte[] { });
        }
    }
}
