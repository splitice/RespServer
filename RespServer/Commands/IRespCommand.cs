using System.Collections.Generic;
using RespServer.Protocol;

namespace RespServer.Commands
{
    public interface IRespCommand
    {
        IEnumerable<RespPart> Execute(RespConnection connection);
    }
}
