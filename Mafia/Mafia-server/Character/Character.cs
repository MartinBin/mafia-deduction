using Mafia_server.Decorator;
using Mafia_server.Strategy;

public abstract class Character:IDecorator
{
    private List<IAbilityStrategy> abilities = new List<IAbilityStrategy>();
    public bool CanUseEvilChat { get; set; }
    public bool CanUseGoodChat { get; set; }

    public void ExecuteTurn(string time)
    {
        if (time == "night")
        {
            PerformMainAction();
        }else if (time == "vote")
        {
            UseAbility("vote");
        }
        else
        {
            sayToChat();
        }
        
    }
    
    protected abstract void PerformMainAction();
    protected abstract void sayToChat();
    public void AddAbility(IAbilityStrategy ability)
    {
        abilities.Add(ability);
    }

    public void UseAbility(string abilityName)
    {
        foreach (var ability in abilities)
        {   if(ability.GetType().Name == abilityName)
            ability.UseAbility();
        }
    }
    public virtual void Render()
    {
        // Basic render functionality
        Console.WriteLine($"Character Role: {CanUseEvilChat}");
    }
}