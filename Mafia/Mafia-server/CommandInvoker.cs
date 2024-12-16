using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mafia_server.Command
{
    public class CommandInvoker
    {
        private readonly Dictionary<string, Func<string[], ICommand>> _commands = new Dictionary<string, Func<string[], ICommand>>();

        public void RegisterCommand(string commandName, Func<string[], ICommand> commandFactory)
        {
            if (!_commands.ContainsKey(commandName))
            {
                _commands.Add(commandName, commandFactory);
            }
        }

        public async Task ExecuteCommandAsync(string commandName, params string[] parameters)
        {
            if (_commands.TryGetValue(commandName, out var commandFactory))
            {
                var command = commandFactory(parameters);
                await command.ExecuteAsync();
            }
            else
            {
                throw new InvalidOperationException("Command not found");
            }
        }
    }
}
