using System.Windows.Controls;
using Mafia_client.Prototype;

public class PlayerAvatar : GameElement
{
    public PlayerAvatar(EllipsePrototype prototype)
    {
        element = prototype.Clone();
    }
    public override void Render(Canvas canvas, double x, double y)
    {
        Canvas.SetLeft(element, x - 25); // Center the 50px wide avatar
        Canvas.SetTop(element, y - 25);  // Center the 50px tall avatar
        canvas.Children.Add(element);
    }
}