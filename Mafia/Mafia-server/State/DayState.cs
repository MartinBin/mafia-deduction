using Mafia_server;
using Mafia_server.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class DayState : IGameState
{
    private readonly GameHub _gameHub;
    private int _timeRemaining;
    private readonly Timer _stateTimer;
    private readonly IHubContext<GameHub> _hubContext;
    private static Logger logger = Logger.getInstance;

    public DayState(GameHub gameHub, IHubContext<GameHub> hubContext)
    {
        _gameHub = gameHub;
        _timeRemaining = Constants.DAY_DURATION;
        _stateTimer = new Timer(UpdateTimer, null, 0, 1000);

        var consoleHandler = new ConsoleLoggerHandler();
        var fileHandler = new FileLoggerHandler(AppDomain.CurrentDomain.BaseDirectory);

        consoleHandler.SetNext(fileHandler);
        logger.SetHandlerChain(consoleHandler);
    }

    public void EnterState()
    {
        logger.Log(LogType.Info, "Day Phase Started");
        // Reset any night actions and reveal any night deaths
        ProcessNightResults();
    }

    public void ExitState()
    {
        _stateTimer.Dispose();
        logger.Log(LogType.Info, "Day Phase Ended");
    }

    public void HandlePlayerAction(int playerId, string action, params object[] args)
    {
        switch (action)
        {
            case "StartVote":
                if (CanStartVote())
                {
                    _gameHub.StateContext.TransitionTo(new VotingState(_gameHub, _hubContext));
                }
                break;
            case "Chat":
                HandleDayChat(playerId, args);
                break;
        }
    }

    private async void UpdateTimer(object state)
    {
        _timeRemaining -= 1000;
        await _gameHub.BroadcastTimeRemaining(_timeRemaining);
        
        if (_timeRemaining <= 0)
        {
            await _gameHub.StateContext.TransitionTo(new VotingState(_gameHub, _hubContext));
            _stateTimer.Dispose();
        }
    }

    public async Task UpdateState()
    {
        await _gameHub.Clients.All.SendAsync("StateChanged", "Day");
        await _gameHub.Clients.All.SendAsync("TimeRemaining", _timeRemaining);
    }

    private async void DayTimeExpired(object state)
    {
        await _gameHub.StateContext.TransitionTo(new VotingState(_gameHub, _hubContext));
    }

    private void ProcessNightResults()
    {
        // Process any deaths or other night phase results
        // Notify players of results
    }

    private bool CanStartVote()
    {
        // Add logic to determine if voting can start
        return true;
    }

    private async void HandleDayChat(int playerId, object[] args)
    {
        if (args.Length > 0 && args[0] is string message)
        {
            await _gameHub.Clients.All.SendAsync("ChatMessage", playerId, message, "Day");
        }
    }
}