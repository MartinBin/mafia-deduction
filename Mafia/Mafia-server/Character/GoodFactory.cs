public class GoodFactory : CharacterFactory
{
    public override Character CreateCharacter()
    {
        // Randomly choose between Good Goon, Good Spy, and Medic
        Random random = new Random();
        int choice = random.Next(3);
        switch (choice)
        {
            case 0:
                return new GoodGoon();
            case 1:
                return new GoodSpy();
            case 2:
                return new Medic();
            default:
                throw new InvalidOperationException("Invalid choice");
        }
    }
}