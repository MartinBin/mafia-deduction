namespace Mafia_server;

public class Player
{
    public int ID { get; private set; }
    public Character Character { get; private set; }
    public bool IsAlive { get; set; } = true;

    public Player(int _ID)
    {
        ID = _ID;
    }

    public void AssignCharacter(Character character)
    {
        Character = character;
    }

    public PlayerMemento SaveState()
    {
        return new PlayerMemento(ID, Character.GetType().Name, IsAlive);
    }

    public void RestoreState(PlayerMemento memento)
    {
        ID = memento.PlayerId;
        // Assuming you have a method to get a character by type
        //Character = CharacterFactory.GetCharacterByType(memento.CharacterType);
        IsAlive = memento.IsAlive;
    }
}
