using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Mafia_client.Factory;

public class Cat : IPet
{
    public Image GetPetImage()
    {
        return new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/Images/cat.png")),
            Width = 50,
            Height = 50
        };
    }
}