namespace Mafia_server.Observer;

public class TerminalCommandHandler
{
    private readonly CommandSubject _commandSubject;
    
    public TerminalCommandHandler(CommandSubject commandSubject)
    {
        _commandSubject = commandSubject;
    }

    public async Task ReadCommandsFromTerminalAsync()
    {
        while (true)
        {
            string commandInput = await ReadCommandAsync();
            if (!string.IsNullOrWhiteSpace(commandInput))
            {
                var commandParts = commandInput.Split(' ');
                var command = commandParts[0];
                var parameters = commandParts.Length > 1 ? commandParts.Skip(1).ToArray() : Array.Empty<string>();

                await _commandSubject.NotifyObserversAsync(command, parameters);
            }
        }
    }

    private async Task<string> ReadCommandAsync()
    {
        await Task.Delay(1000);
        return Console.ReadLine();
    }
}