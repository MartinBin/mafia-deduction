using Mafia_server;

public class GameStateContext
{
    private IGameState _currentState;
    private readonly GameHub _gameHub;
    
    public GameStateContext(GameHub gameHub)
    {
        _gameHub = gameHub;
        _currentState = new LobbyState(_gameHub);
    }

    public async Task TransitionTo(IGameState state)
    {
        _currentState?.ExitState();
        _currentState = state;
        _currentState.EnterState();
        await _currentState.UpdateState();
    }

    public void HandlePlayerAction(int playerId, string action, params object[] args)
    {
        _currentState?.HandlePlayerAction(playerId, action, args);
    }

    public IGameState GetCurrentState()
    {
        return _currentState;
    }
}