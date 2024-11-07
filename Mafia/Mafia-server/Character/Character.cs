using Mafia_server.Decorator;
using Mafia_server.Strategy;

public abstract class Character:IDecorator
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
    public virtual void Render()
    {
        // Basic render functionality
        Console.WriteLine($"Character Role: {CanUseEvilChat}");
    }
}