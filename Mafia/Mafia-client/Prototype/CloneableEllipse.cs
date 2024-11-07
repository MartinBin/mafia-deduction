using System.Windows.Media;
using System.Windows.Shapes;

namespace Mafia_client.Prototype;

public class EllipsePrototype : ICloneable<Ellipse>
{
    private readonly Ellipse _ellipse;

    public EllipsePrototype(double width, double height, Brush fill, Brush stroke, double strokeThickness)
    {
        _ellipse = new Ellipse
        {
            Width = width,
            Height = height,
            Fill = fill,
            Stroke = stroke,
            StrokeThickness = strokeThickness
        };
    }

    public Ellipse Clone()
    {
        return new Ellipse
        {
            Width = _ellipse.Width,
            Height = _ellipse.Height,
            Fill = _ellipse.Fill,
            Stroke = _ellipse.Stroke,
            StrokeThickness = _ellipse.StrokeThickness
        };
    }
}