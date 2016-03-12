using System.Collections.Generic;
using RespServer.Protocol;

namespace RespServer.Commands
{
    public interface ICommand
    {
        IEnumerable<RespPart> Execute();
    }
}
