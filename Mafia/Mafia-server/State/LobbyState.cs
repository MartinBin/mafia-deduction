using Mafia_server;
using Mafia_server.Log;
using Microsoft.AspNetCore.SignalR;

public class LobbyState : IGameState
{
    private readonly GameHub _gameHub;
    private static Logger logger = Logger.getInstance;

    public LobbyState(GameHub gameHub)
    {
        _gameHub = gameHub;

        // Set up logger
        var consoleHandler = new ConsoleLoggerHandler();
        var fileHandler = new FileLoggerHandler(AppDomain.CurrentDomain.BaseDirectory);

        consoleHandler.SetNext(fileHandler);
        logger.SetHandlerChain(consoleHandler);
    }

    public void EnterState()
    {
        logger.Log(LogType.Info, "Entering Lobby State");
    }

    public void ExitState()
    {
        logger.Log(LogType.Info, "Exiting Lobby State");
    }

    public void HandlePlayerAction(int playerId, string action, params object[] args)
    {
        switch (action)
        {
            case "StartGame":
                if (CanStartGame())
                {
                    _gameHub.StartGame();
                }
                break;
            // Handle other lobby actions
        }
    }

    public async Task UpdateState()
    {
        await _gameHub.Clients.All.SendAsync("StateChanged", "Lobby");
    }

    private bool CanStartGame()
    {
        return Globals.clients.Count >= Constants.MIN_PLAYERS_TO_START;
    }
}