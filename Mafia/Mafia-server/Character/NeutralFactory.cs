public class NeutralFactory : CharacterFactory
{
    public override Character CreateCharacter()
    {
        return new Jester();
    }
}