namespace Mafia_server.Command;

public class StartGameCommand : ICommand
{
    private readonly GameHub _gameHub;

    public StartGameCommand(GameHub gameHub)
    {
        _gameHub = gameHub;
    }

    public async Task ExecuteAsync()
    {
        await _gameHub.StartGame();
    }
}