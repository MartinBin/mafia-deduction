using Mafia_server;
using Mafia_server.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class EndGameState : IGameState
{
    private readonly GameHub _gameHub;
    private string _winningTeam;
    private static Logger logger = Logger.getInstance;

    public EndGameState(GameHub gameHub)
    {
        _gameHub = gameHub;

        var consoleHandler = new ConsoleLoggerHandler();
        var fileHandler = new FileLoggerHandler(AppDomain.CurrentDomain.BaseDirectory);

        consoleHandler.SetNext(fileHandler);
        logger.SetHandlerChain(consoleHandler);
    }

    public void EnterState()
    {
        logger.Log(LogType.Info, "Game Ended");
        DetermineWinner();
    }

    public void ExitState()
    {
        // Clean up game state
        ResetGame();
    }

    public void HandlePlayerAction(int playerId, string action, params object[] args)
    {
        switch (action)
        {
            case "ReturnToLobby":
                _gameHub.StateContext.TransitionTo(new LobbyState(_gameHub));
                break;
        }
    }

    public async Task UpdateState()
    {
        await _gameHub.Clients.All.SendAsync("StateChanged", "GameEnd");
        await _gameHub.Clients.All.SendAsync("GameResults", _winningTeam);
    }

    private void DetermineWinner()
    {
        // Count remaining players and determine winning team
        var players = Globals.clients.Values.Select(c => c.Player).ToList();
        var evilPlayers = players.Count(p => p.Character.CanUseEvilChat);
        var goodPlayers = players.Count(p => !p.Character.CanUseEvilChat);

        _winningTeam = evilPlayers >= goodPlayers ? "Evil" : "Good";
    }

    private void ResetGame()
    {
        // Reset all game-related variables
        foreach (var client in Globals.clients.Values)
        {
            client.Player = new Player(client.PlayerID);
        }
    }
}