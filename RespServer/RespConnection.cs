using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mina.Core.Future;
using Mina.Core.Session;
using RespServer.Commands;
using RespServer.Protocol;

namespace RespServer
{
    public class RespConnection
    {
        private IoSession _socket;
        public event EventHandler<EventArgs> OnDisconnect; 
        private RespCommandRegistry _respCommands;
        private RespParser _parser = new RespParser();

        public IoSession Socket
        {
            get { return _socket; }
        }

        public RespConnection(IoSession socket, RespCommandRegistry respCommands)
        {
            _socket = socket;
            _respCommands = respCommands;
        }

        internal void HandleDisconnect()
        {
            if (OnDisconnect != null) OnDisconnect(this, new EventArgs());
        }

        private IEnumerable<RespPart> HandleCommand(List<object> response)
        {
            if (response.Count != 0)
            {
                var commandName = response[0] as byte[];
                if (commandName == null)
                {
                    return new List<RespPart> {RespPart.Error("The command must be supplied as a string")};
                }
                var commandString = Encoding.ASCII.GetString(commandName);
                var command = _respCommands.NewCommand(commandString, response.Skip(1).ToList());
                if (command == null)
                {
                    return new List<RespPart>
                    {
                        RespPart.Error(String.Format("Command {0} not found", commandString))
                    };
                }
                try
                {
                    return command.Execute(this);
                }
                catch (Exception ex)
                {
                    return new List<RespPart>
                    {
                        RespPart.String(String.Format("An Exception Occured: {0}", ex))
                    };
                }
            }
            return null;
        }

        public void HandleMessage(RespPart message)
        {
            IEnumerable<RespPart> outputParts = null;
            try
            {
                var arguments = _parser.MessageHandle(message);
                if (arguments != null)
                {
                    outputParts = HandleCommand(arguments);
                }
            }
            catch (Exception ex)
            {
                outputParts = new List<RespPart> { RespPart.Error(ex.Message) };
            }

            if (outputParts != null)
            {
                foreach (var output in outputParts)
                {
                    _socket.Write(output);
                }
            }
        }
    }
}
