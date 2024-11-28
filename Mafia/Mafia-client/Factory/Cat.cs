using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Mafia_client.Factory;

public class Cat : IPet
{
    public PetSkin Skin { get; }

    public Cat(PetSkin skin)
    {
        Skin = skin;
    }

    public Image GetPetImage()
    {
        return Skin.Image;
    }
}