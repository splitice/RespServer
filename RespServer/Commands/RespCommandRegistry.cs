using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespServer.Commands
{
    public class RespCommandRegistry
    {
        private Dictionary<String, Func<List<object>, IRespCommand>> _commands = new Dictionary<string, Func<List<object>, IRespCommand>>();
        public IRespCommand NewCommand(String name, List<object> arguments)
        {
            Func<List<object>, IRespCommand> command;
            if (_commands.TryGetValue(name.ToUpperInvariant(), out command))
            {
                return command(arguments);
            }
            return null;
        }

        public void RegisterCommand(String name, Func<List<object>, IRespCommand> creation)
        {
            _commands.Add(name.ToUpperInvariant(), creation);
        }
    }
}
