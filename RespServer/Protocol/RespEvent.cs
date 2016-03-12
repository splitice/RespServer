using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespServer.Protocol
{
    class RespEvent: EventArgs
    {
        public List<object> Arguments;
        public Exception Exception;
    }
}
