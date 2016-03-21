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
            if (marker.Type == RespMarker.MarkerType.Empty)
            {
                return MessageDecoderResult.OK;
            }

            var readLength = marker.ReadLength;
            if (readLength != 0 && input.Remaining < readLength + 1)
            {
                return MessageDecoderResult.NeedData;
            }

            return MessageDecoderResult.OK;
        }

        private RespMarker Parse(Func<byte> input, out byte[] header)
        {
            byte type = input();
            if (type == '\r' || type == '\n')
            {
                header = null;
                return new RespMarker(RespMarker.MarkerType.Empty, 0);
            }

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
            if (marker.Type == RespMarker.MarkerType.Empty)
            {
                return MessageDecoderResult.OK;
            }

            IEnumerable<byte> body;
            if (marker.ReadLine)
            {
                body = line;
            }
            else
            {
                if (marker.ReadLength == 0)
                {
                    body = new byte[0];
                }
                else
                {
                    body = input.GetSlice(marker.ReadLength).GetRemaining();

                    byte nl = input.Get();
                    if (nl != '\r' && nl != '\n')
                    {
                        return MessageDecoderResult.NotOK;
                    }
                }
            }

            output.Write(new RespPart(marker, body));

            return MessageDecoderResult.OK;
        }

        public void FinishDecode(IoSession session, IProtocolDecoderOutput output)
        {
        }
    }
}
