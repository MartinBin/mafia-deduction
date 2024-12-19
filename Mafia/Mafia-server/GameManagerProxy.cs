namespace Mafia_server
{
    public class GameManagerProxy : IGameManager
    {
        private IGameManager _gameManager;
        private bool isAccessible;
        public GameManagerProxy(IGameManager gameManager)
        {
            _gameManager = gameManager;
            isAccessible = true;
        }

        public void GiveAccess()
        {
            isAccessible = true;
        }

        public void RemoveAccess()
        {
            isAccessible = false;
        }

        private bool CheckAccess()
        {
            return isAccessible;
        }

        public void AssignCharactersToPlayers(List<Player> players)
        {
            if (CheckAccess())
            {
                _gameManager.AssignCharactersToPlayers(players);
            }
            
        }
    }
}
