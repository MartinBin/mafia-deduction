using System.Windows.Controls;
using Mafia_client.Prototype;
using Mafia_client.Visitor;

public class PlayerRoleLabel : GameElement
{
    public PlayerRoleLabel(TextBlockPrototype prototype, string text)
    {
        var textBlock = prototype.Clone();
        textBlock.Text = text;
        element = textBlock;
    }

    public override void Render(Canvas canvas, double x, double y)
    {
        Canvas.SetLeft(element, x - 50); // Center the 100px wide label
        Canvas.SetTop(element, y - 80);  // Place above name label
        canvas.Children.Add(element);
    }
    
    public override void Accept(IGameComponentVisitor visitor)
    {
        visitor.Visit(this);
    }
}