using System.Collections.Generic;
using RespServer.Protocol;

namespace RespServer.Commands
{
    interface ICommand
    {
        IEnumerable<RespPart> Execute();
    }
}
