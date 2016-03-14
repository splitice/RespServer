using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespServer.Commands
{
    public class CommandRegistry
    {
        private Dictionary<String, Func<List<object>, ICommand>> _commands = new Dictionary<string, Func<List<object>, ICommand>>();
        public ICommand NewCommand(String name, List<object> arguments)
        {
            Func<List<object>, ICommand> command;
            if (_commands.TryGetValue(name.ToUpperInvariant(), out command))
            {
                return command(arguments);
            }
            return null;
        }

        public void RegisterCommand(String name, Func<List<object>, ICommand> creation)
        {
            _commands.Add(name.ToUpperInvariant(), creation);
        }
    }
}
