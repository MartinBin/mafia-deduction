public interface IGameState
{
    void EnterState();
    void ExitState();
    void HandlePlayerAction(int playerId, string action, params object[] args);
    Task UpdateState();
}