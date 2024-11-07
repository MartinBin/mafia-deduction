using Mafia_server.Command;

namespace Mafia_server;

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
        if (_commands.ContainsKey(commandName))
        {
            var command = _commands[commandName].Invoke(parameters);
            await command.ExecuteAsync();
        }
    }
}