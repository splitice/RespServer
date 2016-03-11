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
        private RespParser _parser = new RespParser();
        private CommandRegistry _commands;

        public RespConnection(ReactiveSocket socket, CommandRegistry commands)
        {
            _socket = socket;
            _commands = commands;

            IObservable<RespPart> messages = from header in socket.Receiver.TakeWhile((a) => a != '\n').ToArray()
                                             let marker = RespProtocolReader.ReadMarker(header.ToArray())
                                             let body = marker.ReadLine ? socket.Receiver.TakeWhile((a) => a != '\n') : socket.Receiver.Take(marker.ReadLength)
                                             select new RespPart(marker, body.ToEnumerable());

            // Echo the incoming message with the same format.
            messages.Subscribe(HandleCommand);
            messages.Catch<RespPart,Exception>(tx => Observable.Empty<RespPart>());

            socket.Disconnected += (a,e) => { if(OnDisconnect != null) OnDisconnect(this, e); };
        }

        private void HandleCommand(RespPart message)
        {
            var response = _parser.MessageHandle(message);

            if (response != null && response.Count != 0)
            {
                IEnumerable<RespPart> outputParts;
                var commandName = response[0] as byte[];
                if (commandName == null)
                {
                    outputParts = new List<RespPart> {RespPart.Error("The command must be supplied as a string")};
                }
                else
                {
                    var commandString = System.Text.Encoding.ASCII.GetString(commandName);
                    var command = _commands.NewCommand(commandString,
                        response.Skip(1).ToList());
                    if (command == null)
                    {
                        outputParts = new List<RespPart> {RespPart.Error(String.Format("Command {0} not found", commandString))};
                    }
                    else
                    {
                        try
                        {
                            outputParts = command.Execute();
                        }
                        catch (Exception ex)
                        {
                            outputParts = new List<RespPart> {RespPart.Error(String.Format("An Exception Occured: {0}", ex))};
                        }
                    }
                }

                foreach (var output in outputParts)
                {
                    _socket.SendAsync(output.Serialize()).Wait();
                }
            }
        }
    }
}
