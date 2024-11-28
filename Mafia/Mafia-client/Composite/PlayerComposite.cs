using System.Windows.Controls;

public class PlayerComposite : IGameComponent
{
    private List<IGameComponent> _components = new List<IGameComponent>();
    private double _offsetX;
    private double _offsetY;

    public PlayerComposite(double offsetX = 0, double offsetY = 0)
    {
        _offsetX = offsetX;
        _offsetY = offsetY;
    }

    public void AddComponent(IGameComponent component)
    {
        _components.Add(component);
    }
    public T GetComponent<T>() where T : class, IGameComponent
    {
        return _components.OfType<T>().FirstOrDefault();
    }

    public void Render(Canvas canvas, double x, double y)
    {
        foreach (var component in _components)
        {
            component.Render(canvas, x + _offsetX, y + _offsetY);
        }
    }

    public void Update()
    {
        foreach (var component in _components)
        {
            component.Update();
        }
    }
}