using System.Windows;
using System.Windows.Controls;
using Mafia_client.Visitor;


public abstract class GameElement : IGameComponent
{
    protected internal UIElement element;
    
    public virtual void Render(Canvas canvas, double x, double y)
    {
        if (element != null)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            canvas.Children.Add(element);
        }
    }
    
    public virtual void Update() { }
    public abstract void Accept(IGameComponentVisitor visitor);
}