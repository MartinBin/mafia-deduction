namespace Mafia_server.Decorator;

public abstract class CharacterDecorator : IDecorator
{
    protected IDecorator _decoratedCharacter;
    
    public CharacterDecorator(IDecorator decoratedCharacter)
    {
        _decoratedCharacter = decoratedCharacter;
    }

    public virtual void Render()
    {
        _decoratedCharacter.Render();
    }
}