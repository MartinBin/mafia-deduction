namespace Mafia_server
{
    public class GameManagerProxy : IGameManager
    {
        private IGameManager _gameManager;
        public GameManagerProxy(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void AssignCharactersToPlayers(List<Player> players)
        {
            _gameManager.AssignCharactersToPlayers(players);
        }
    }
}
