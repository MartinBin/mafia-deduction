namespace Mafia_server.Decorator;

public class ImageDecorator : CharacterDecorator
{
    private readonly string _imagePath;

    public ImageDecorator(IDecorator decoratedCharacter, string imagePath) : base(decoratedCharacter)
    {
        _imagePath = imagePath;
    }

    public override void Render()
    {
        base.Render();
        AddImage();
    }

    private void AddImage()
    {
        // Logic to add image using _imagePath
        Console.WriteLine($"Adding character image from path: {_imagePath}");
    }
}