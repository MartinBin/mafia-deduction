using System.Windows.Controls;

public interface IGameComponent
{
    void Render(Canvas canvas, double x, double y);
    void Update();
}