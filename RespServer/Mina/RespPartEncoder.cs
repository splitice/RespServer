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
    class RespPartEncoder : IMessageEncoder<RespPart>
    {
        public void Encode(IoSession session, RespPart message, IProtocolEncoderOutput output)
        {
            IoBuffer buf = IoBuffer.Wrap(message.Serialize());
            output.Write(buf);
        }

        public void Encode(IoSession session, object message, IProtocolEncoderOutput output)
        {
            Encode(session, message as RespPart, output);
        }
    }
}
