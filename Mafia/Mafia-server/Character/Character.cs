using Mafia_server.Strategy;

public abstract class Character
{
    private List<IAbilityStrategy> abilities = new List<IAbilityStrategy>();
    public bool CanUseEvilChat { get; set; }
    public bool CanUseGoodChat { get; set; }
    public void AddAbility(IAbilityStrategy ability)
    {
        abilities.Add(ability);
    }

    public void UseAbilities()
    {
        foreach (var ability in abilities)
        {
            ability.UseAbility();
        }
    }
}