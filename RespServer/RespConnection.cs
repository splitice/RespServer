using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveSockets;
using RespServer.Commands;
using RespServer.Protocol;

namespace RespServer
{
    class RespConnection
    {
        private ReactiveSocket _socket;
        public event EventHandler<EventArgs> OnDisconnect; 
        private CommandRegistry _commands;
        private RespCommunicator _communicator;

        public RespConnection(ReactiveSocket socket, CommandRegistry commands)
        {
            _socket = socket;
            _commands = commands;
            _communicator = new RespCommunicator(socket.Receiver);
            _communicator.MessageArrived += HandleCommand;
            socket.Disconnected += (a, e) => { if (OnDisconnect != null) OnDisconnect(this, e); };
            _communicator.Start();
        }

        private void HandleCommand(object sender, RespEvent respEvent)
        {
            IEnumerable<RespPart> outputParts = null;
            if (respEvent.Exception != null)
            {
                outputParts = new List<RespPart> { RespPart.Error(respEvent.Exception.Message) };
            }
            else
            {
                List<object> response = respEvent.Arguments;
                if (response.Count != 0)
                {
                    var commandName = response[0] as byte[];
                    if (commandName == null)
                    {
                        outputParts = new List<RespPart> {RespPart.Error("The command must be supplied as a string")};
                    }
                    else
                    {
                        var commandString = Encoding.ASCII.GetString(commandName);
                        var command = _commands.NewCommand(commandString,
                            response.Skip(1).ToList());
                        if (command == null)
                        {
                            outputParts = new List<RespPart>
                            {
                                RespPart.Error(String.Format("Command {0} not found", commandString))
                            };
                        }
                        else
                        {
                            try
                            {
                                outputParts = command.Execute();
                            }
                            catch (Exception ex)
                            {
                                outputParts = new List<RespPart>
                                {
                                    RespPart.String(String.Format("An Exception Occured: {0}", ex))
                                };
                            }
                        }
                    }
                }
            }

            if (outputParts != null)
            {
                foreach (var output in outputParts)
                {
                    _socket.SendAsync(output.Serialize()).Wait();
                }
            }
        }
    }
}
