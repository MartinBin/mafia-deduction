using Mafia_server;
using Microsoft.AspNetCore.SignalR;

public class NightState : IGameState
{
    private readonly GameHub _gameHub;
    private readonly Timer _stateTimer;
    private int _timeRemaining;
    private readonly IHubContext<GameHub> _hubContext;

    public NightState(GameHub gameHub, IHubContext<GameHub> hubContext)
    {
        _gameHub = gameHub;
        _timeRemaining = Constants.NIGHT_DURATION;
        _stateTimer = new Timer(UpdateTimer, null, 0, 1000);
    }
    
    private async void UpdateTimer(object state)
    {
        _timeRemaining -= 1000;
        await _gameHub.BroadcastTimeRemaining(_timeRemaining);
        
        if (_timeRemaining <= 0)
        {
            await _gameHub.StateContext.TransitionTo(new DayState(_gameHub, _hubContext));
            _stateTimer.Dispose();
        }
    }

    public void EnterState()
    {
        Logger.getInstance.Log(LogType.Info, "Night Phase Started");
    }

    public void ExitState()
    {
        _stateTimer.Dispose();
        Logger.getInstance.Log(LogType.Info, "Night Phase Ended");
    }

    public void HandlePlayerAction(int playerId, string action, params object[] args)
    {
        // Handle night actions (killing, healing, etc.)
    }

    public async Task UpdateState()
    {
        await _gameHub.Clients.All.SendAsync("StateChanged", "Night");
    }

    /*private async void NightTimeExpired(object state)
    {
        await _gameHub.StateContext.TransitionTo(new DayState(_gameHub));
    }*/
}