namespace Mafia_server.Decorator;

public class LabelDecorator : CharacterDecorator
{
    private readonly string _label;

    public LabelDecorator(IDecorator decoratedCharacter, string label) : base(decoratedCharacter)
    {
        _label = label;
    }

    public override void Render()
    {
        base.Render();
        AddLabel();
    }

    private void AddLabel()
    {
        // Logic to add label
        Console.WriteLine($"Adding character label: {_label}");
    }
}