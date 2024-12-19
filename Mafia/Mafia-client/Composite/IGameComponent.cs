using System.Windows.Controls;
using Mafia_client.Visitor;

public interface IGameComponent
{
    void Render(Canvas canvas, double x, double y);
    void Update();
    void Accept(IGameComponentVisitor visitor);
}