using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mafia_client.Visitor;

public class DeathEffectVisitor : IGameComponentVisitor
{
    public void Visit(PlayerAvatar avatar)
    {
        if (avatar.element is Ellipse ellipse)
        {
            ellipse.Fill = new SolidColorBrush(Colors.Gray);
            ellipse.Opacity = 0.5;
        }
    }

    public void Visit(PlayerLabel label)
    {
        if (label.element is TextBlock text)
        {
            text.Foreground = new SolidColorBrush(Colors.Gray);
        }
    }

    public void Visit(PlayerRoleLabel roleLabel) 
    {
        if (roleLabel.element is TextBlock text)
        {
            text.Foreground = new SolidColorBrush(Colors.Gray);
        }
    }

    public void Visit(PlayerPet pet)
    {
        if (pet.element is Image image)
        {
            image.Opacity = 0.5;
        }
    }

    public void Visit(PlayerComposite composite) { }
}