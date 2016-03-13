using System;
using System.Collections.Generic;
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

            var marker = Parse(chars.Dequeue);

            if (input.Remaining < marker.ReadLength)
            {
                return MessageDecoderResult.NeedData;
            }

            return MessageDecoderResult.OK;
        }

        private RespMarker Parse(Func<byte> input)
        {
            var header = new StringBuilder();
            byte type = input();
            byte i;

            do
            {
                i = input();
                header.Append((char)i);
            } while (i != '\n');

            int headerEnd;
            if (header[header.Length - 2] == '\r')
            {
                headerEnd = header.Length - 2;
            }
            else
            {
                headerEnd = header.Length - 1;
            }

            return RespMarker.ReadMarker(type, header.ToString(0, headerEnd));
        }

        public MessageDecoderResult Decode(IoSession session, IoBuffer input, IProtocolDecoderOutput output)
        {
            var marker = Parse(() =>
            {
                return input.Get();
            });

            IEnumerable<byte> body;
            if (marker.ReadLine)
            {
                body = new byte[] { };
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
