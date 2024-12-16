using System;
using System.Threading.Tasks;

namespace Mafia_server.Interpreter
{
    public class CommandInterpreter
    {
        private readonly GameHub _gameHub;

        public CommandInterpreter(GameHub gameHub)
        {
            _gameHub = gameHub;
        }

        public async Task InterpretAsync(string command)
        {
            var parts = command.Split(' ');
            ICommand commandObject = parts[0] switch
            {
                "send" => new SendMessageCommand(_gameHub, parts[1], string.Join(' ', parts[2..])),
                "start" => new StartGameCommand(_gameHub),
                _ => throw new InvalidOperationException("Unknown command")
            };

            await commandObject.ExecuteAsync();
        }
    }
}