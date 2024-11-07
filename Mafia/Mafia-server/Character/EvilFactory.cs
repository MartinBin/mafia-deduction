using Mafia_server.Builder;
using Mafia_server.Strategy;

public class EvilFactory : CharacterFactory
{
    public override Character CreateCharacter()
    {
        Random random = new Random();
        int choice = random.Next(2);
        Character character;
        switch (choice)
        {
            case 0:
                character = new EvilGoon();
                break;
            case 1:
                character = new EvilSpy();
                break;
            default:
                throw new InvalidOperationException("Invalid choice");
        }
        
        ICharacterBuilder builder = new CharacterBuilder(character)
            .SetCanUseEvilChat(true)
            .AddAbility(new KillStrategy())
            .AddAbility(new VoteStrategy());
        
        if (choice == 1)
        {
            builder.AddAbility(new SpyStrategy());
        }
        return builder.Build();
    }
}