namespace Mafia_server
{
    public class PlayerMemento
    {
        public int PlayerId { get; }
        public string CharacterType { get; }
        public bool IsAlive { get; }

        public PlayerMemento(int playerId, string characterType, bool isAlive)
        {
            PlayerId = playerId;
            CharacterType = characterType;
            IsAlive = isAlive;
        }
    }
}