using Mafia_server.Builder;
using Mafia_server.Strategy;

public class NeutralFactory : CharacterFactory
{
    public override Character CreateCharacter()
    {
        Character character;
        character = new Jester();
        ICharacterBuilder builder = new CharacterBuilder(character)
            .SetCanUseGoodChat(true)
            .AddAbility(new TauntStrategy())
            .AddAbility(new VoteStrategy());
        return builder.Build();
    }
}