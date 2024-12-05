using Mafia_server;
using Mafia_server.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class NightState : IGameState
{
    private readonly GameHub _gameHub;
    private readonly Timer _stateTimer;
    private int _timeRemaining;
    private readonly IHubContext<GameHub> _hubContext;
    private static Logger logger = Logger.getInstance;

    public NightState(GameHub gameHub, IHubContext<GameHub> hubContext)
    {
        _gameHub = gameHub;
        _timeRemaining = Constants.NIGHT_DURATION;
        _stateTimer = new Timer(UpdateTimer, null, 0, 1000);

        var consoleHandler = new ConsoleLoggerHandler();
        var fileHandler = new FileLoggerHandler(AppDomain.CurrentDomain.BaseDirectory);

        consoleHandler.SetNext(fileHandler);
        logger.SetHandlerChain(consoleHandler);
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
        logger.Log(LogType.Info, "Night Phase Started");
    }

    public void ExitState()
    {
        _stateTimer.Dispose();
        logger.Log(LogType.Info, "Night Phase Ended");
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