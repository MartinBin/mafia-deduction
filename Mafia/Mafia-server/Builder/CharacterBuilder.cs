using Mafia_server.Strategy;

namespace Mafia_server.Builder;

public class CharacterBuilder : ICharacterBuilder
{
    private Character character;

    public CharacterBuilder(Character character)
    {
        this.character = character;
    }

    public ICharacterBuilder AddAbility(IAbilityStrategy ability)
    {
        character.AddAbility(ability);
        return this;
    }

    public ICharacterBuilder SetCanUseEvilChat(bool canUseEvilChat)
    {
        character.CanUseEvilChat = canUseEvilChat;
        return this;
    }

    public ICharacterBuilder SetCanUseGoodChat(bool canUseGoodChat)
    {
        character.CanUseGoodChat = canUseGoodChat;
        return this;
    }

    public Character Build()
    {
        return character;
    }
}