using System;
using System.Threading.Tasks;
using Mafia_server.Command;

namespace Mafia_server.Interpreter
{
    public class CommandInterpreter
    {
        private readonly CommandInvoker _commandInvoker;

        public CommandInterpreter(CommandInvoker commandInvoker)
        {
            _commandInvoker = commandInvoker;
        }

        public async Task InterpretAsync(string command)
        {
            try
            {
                var parts = command.Split(' ');
                var commandName = parts[0];
                var parameters = parts[1..];

                await _commandInvoker.ExecuteCommandAsync(commandName, parameters);
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific command not found exception
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
