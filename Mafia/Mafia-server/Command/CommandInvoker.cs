using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mafia_server.Command
{
    public class CommandInvoker
    {
        private readonly Dictionary<string, Func<string[], ICommand>> _commands = new Dictionary<string, Func<string[], ICommand>>();

        public void RegisterCommand(string commandName, Func<string[], ICommand> command)
        {
            _commands[commandName] = command;
        }

        public async Task ExecuteCommandAsync(string commandName, params string[] parameters)
        {
            if (_commands.TryGetValue(commandName, out var command))
            {
                var commandInstance = command(parameters);
                await commandInstance.ExecuteAsync();
            }
            else
            {
                throw new InvalidOperationException("Command not found");
            }
        }
    }
}