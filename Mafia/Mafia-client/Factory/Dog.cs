using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Mafia_client.Factory;

public class Dog : IPet
{
    public PetSkin Skin { get; }

    public Dog(PetSkin skin)
    {
        Skin = skin;
    }

    public Image GetPetImage()
    {
        return Skin.Image;
    }
}