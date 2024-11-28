using System.Windows.Controls;
using System.Windows.Media.Imaging;

public class PetSkin
{
    public string FileName { get; }
    public Image Image { get; }

    public PetSkin(string fileName)
    {
        FileName = fileName;
        Image = new Image
        {
            Source = new BitmapImage(new Uri($"pack://application:,,,/Images/Pets/{fileName}")),
            Width = 30,
            Height = 30
        };
    }
}