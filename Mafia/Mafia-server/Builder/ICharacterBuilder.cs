using Mafia_server.Strategy;

namespace Mafia_server.Builder;

public interface ICharacterBuilder
{
    ICharacterBuilder AddAbility(IAbilityStrategy ability);
    ICharacterBuilder SetCanUseEvilChat(bool canUseEvilChat);
    ICharacterBuilder SetCanUseGoodChat(bool canUseGoodChat);
    Character Build();
}