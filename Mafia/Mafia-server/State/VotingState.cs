using Mafia_server;
using Mafia_server.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class VotingState : IGameState
{
    private readonly GameHub _gameHub;
    private readonly Timer _stateTimer;
    private int _timeRemaining;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly Dictionary<int, int> _votes = new();
    private static Logger logger = Logger.getInstance;

    public VotingState(GameHub gameHub, IHubContext<GameHub> hubContext)
    {
        _gameHub = gameHub;
        _hubContext = hubContext;
        _timeRemaining = Constants.VOTING_DURATION;
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
            ProcessVotingResults();
            if (CheckGameEnd())
            {
                await _gameHub.StateContext.TransitionTo(new EndGameState(_gameHub));
            }
            else
            {
                await _gameHub.StateContext.TransitionTo(new NightState(_gameHub, _hubContext));
            }
            _stateTimer.Dispose();
        }
    }

    public void EnterState()
    {
        logger.Log(LogType.Info, "Voting Phase Started");
        _votes.Clear();
    }

    public void ExitState()
    {
        _stateTimer.Dispose();
        logger.Log(LogType.Info, "Voting Phase Ended");
    }

    public void HandlePlayerAction(int playerId, string action, params object[] args)
    {
        switch (action)
        {
            case "Vote":
                if (args.Length > 0 && args[0] is int targetId)
                {
                    HandleVote(playerId, targetId);
                }
                break;
        }
    }

    public async Task UpdateState()
    {
        await _gameHub.Clients.All.SendAsync("StateChanged", "Voting");
        await _gameHub.Clients.All.SendAsync("TimeRemaining", _timeRemaining);
    }

    private async void VotingTimeExpired(object state)
    {
        ProcessVotingResults();
        
        if (CheckGameEnd())
        {
            await _gameHub.StateContext.TransitionTo(new EndGameState(_gameHub));
        }
        else
        {
            await _gameHub.StateContext.TransitionTo(new NightState(_gameHub, _hubContext));
        }
    }

    private async void HandleVote(int voterId, int targetId)
    {
        if (IsValidVote(voterId, targetId))
        {
            _votes[voterId] = targetId;
            await _gameHub.Clients.All.SendAsync("VoteRegistered", voterId, targetId);
        }
    }

    private void ProcessVotingResults()
    {
        // Count votes and eliminate player if necessary
        var voteCounts = _votes.GroupBy(v => v.Value)
                              .ToDictionary(g => g.Key, g => g.Count());

        if (voteCounts.Any())
        {
            var mostVoted = voteCounts.OrderByDescending(v => v.Value).First();
            // Eliminate player with most votes
            EliminatePlayer(mostVoted.Key);
        }
    }

    private bool IsValidVote(int voterId, int targetId)
    {
        return Globals.clients.ContainsKey(voterId) && 
               Globals.clients.ContainsKey(targetId) && 
               !IsPlayerDead(voterId);
    }

    private bool IsPlayerDead(int playerId)
    {
        // Add logic to check if player is dead
        return false;
    }

    private void EliminatePlayer(int playerId)
    {
        // Add logic to eliminate player
    }

    private bool CheckGameEnd()
    {
        // Add win condition checks
        return false;
    }
}