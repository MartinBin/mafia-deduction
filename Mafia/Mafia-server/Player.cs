namespace Mafia_server;

public class Player
{
    public int ID { get; private set; }
    public Character Character { get; private set; }

    public Player(int _ID)
    {
        ID = _ID;
    }

    public void AssignCharacter(Character character)
    {
        Character = character;
    }
}
