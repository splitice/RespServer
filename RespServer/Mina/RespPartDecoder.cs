using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mina.Core.Buffer;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.Demux;
using RespServer.Protocol;

namespace RespServer.Mina
{
    class RespPartDecoder : IMessageDecoder
    {
        public MessageDecoderResult Decodable(IoSession session, IoBuffer input)
        {
            Queue<byte> chars = new Queue<byte>();
            byte c = 0;
            while (input.HasRemaining)
            {
                c = input.Get();
                chars.Enqueue(c);
                if (c == (byte)'\n')
                {
                    break;
                }
            }

            if (c != (byte)'\n')
            {
                return MessageDecoderResult.NeedData;
            }

            byte[] line;
            var marker = Parse(chars.Dequeue, out line);

            if (input.Remaining < marker.ReadLength)
            {
                return MessageDecoderResult.NeedData;
            }

            return MessageDecoderResult.OK;
        }

        private RespMarker Parse(Func<byte> input, out byte[] header)
        {
            byte type = input();
            byte i;

            using (var ms = new MemoryStream())
            {
                do
                {
                    i = input();
                    ms.WriteByte(i);
                } while (i != '\n');

                ms.Position = 0;
                header = ms.ToArray();
            }

            return RespMarker.ReadMarker(type, header);
        }

        public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            byte[] line;
            var marker = Parse(() =>
            {
                return input.Get();
            }, out line);

            IEnumerable<byte> body;
            if (marker.ReadLine)
            {
                body = line;
            }
            else
            {
                body = input.GetSlice(marker.ReadLength).GetRemaining();
            }

            output.Write(new RespPart(marker, body));

            return MessageDecoderResult.OK;
        }

        public void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
        }
    }
}
