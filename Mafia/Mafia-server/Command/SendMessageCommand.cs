namespace Mafia_server.Command;

public class SendMessageCommand : ICommand
{
    private readonly GameHub _gameHub;
    private readonly string _username;
    private readonly string _message;

    public SendMessageCommand(GameHub gameHub, string username, string message)
    {
        _gameHub = gameHub;
        _username = "Server";
        _message = message;
    }

    public async Task ExecuteAsync()
    {
        await _gameHub.SendMessage(_username, _message);
    }
}