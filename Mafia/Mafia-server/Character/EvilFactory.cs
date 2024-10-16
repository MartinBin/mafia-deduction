public class EvilFactory : CharacterFactory
{
    public override Character CreateCharacter()
    {
        // Randomly choose between Evil Goon and Evil Spy
        Random random = new Random();
        int choice = random.Next(2);
        return choice == 0 ? new EvilGoon() : new EvilSpy();
    }
}