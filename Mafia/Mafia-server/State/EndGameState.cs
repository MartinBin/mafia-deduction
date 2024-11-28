using Mafia_server;
using Microsoft.AspNetCore.SignalR;

public class EndGameState : IGameState
{
    private readonly GameHub _gameHub;
    private string _winningTeam;

    public EndGameState(GameHub gameHub)
    {
        _gameHub = gameHub;
    }

    public void EnterState()
    {
        Logger.getInstance.Log(LogType.Info, "Game Ended");
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