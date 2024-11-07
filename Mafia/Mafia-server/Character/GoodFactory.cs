using Mafia_server.Builder;
using Mafia_server.Strategy;

public class GoodFactory : CharacterFactory
{
    public override Character CreateCharacter()
    {
        // Randomly choose between Good Goon, Good Spy, and Medic
        Random random = new Random();
        int choice = random.Next(3);
        Character character;
        switch (choice)
        {
            case 0:
                character = new GoodGoon();
                break;
            case 1:
                character = new GoodSpy();
                break;
            case 2:
                character = new Medic();
                break;
            default:
                throw new InvalidOperationException("Invalid choice");
        }
        
        ICharacterBuilder builder = new CharacterBuilder(character)
            .SetCanUseGoodChat(true)
            .AddAbility(new VoteStrategy());
        if (choice == 1)
        {
            builder.AddAbility(new ArrestStrategy()).AddAbility(new SpyStrategy());
        }else if (choice == 2)
        {
            builder.AddAbility(new HealStrategy());
        }
        
        return builder.Build();
    }
}