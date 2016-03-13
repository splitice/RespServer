using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mina.Filter.Codec.Demux;
using RespServer.Protocol;

namespace RespServer.Mina
{
    class RespProtocolFactory : DemuxingProtocolCodecFactory
    {
        public RespProtocolFactory()
        {
            AddMessageDecoder<RespPartDecoder>();
            AddMessageEncoder<RespPart, RespPartEncoder>();
        }
    }
}
