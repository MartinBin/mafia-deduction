using Mafia_server;
using Microsoft.AspNetCore.SignalR;

public class LobbyState : IGameState
{
    private readonly GameHub _gameHub;

    public LobbyState(GameHub gameHub)
    {
        _gameHub = gameHub;
    }

    public void EnterState()
    {
        Logger.getInstance.Log(LogType.Info, "Entering Lobby State");
    }

    public void ExitState()
    {
        Logger.getInstance.Log(LogType.Info, "Exiting Lobby State");
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