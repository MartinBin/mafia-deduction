namespace Mafia_server.Decorator;

public class InfoDecorator : CharacterDecorator
{
    private readonly string _info;

    public InfoDecorator(IDecorator decoratedCharacter, string info) : base(decoratedCharacter)
    {
        _info = info;
    }

    public override void Render()
    {
        base.Render();
        AddInfo();
    }

    private void AddInfo()
    {
        // Logic to add additional information
        Console.WriteLine($"Adding character info: {_info}");
    }
}