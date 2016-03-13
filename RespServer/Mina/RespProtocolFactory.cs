using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mina.Filter.Codec.Demux;

namespace RespServer.Mina
{
    class RespProtocolFactory : DemuxingProtocolCodecFactory
    {
        public RespProtocolFactory()
        {
            AddMessageDecoder<RespPartDecoder>();
        }
    }
}
