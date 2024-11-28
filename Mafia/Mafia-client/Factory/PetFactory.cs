namespace Mafia_client.Factory;

public static class PetFactory
{
    private static readonly Dictionary<string, PetSkin> _skins = new();

    private static PetSkin GetPetSkin(string fileName)
    {
        if (!_skins.ContainsKey(fileName))
        {
            _skins[fileName] = new PetSkin(fileName);
        }
        return _skins[fileName];
    }

    public static IPet CreatePet(string type)
    {
        return type.ToLower() switch
        {
            "cat" => new Cat(GetPetSkin("cat.png")),
            "dog" => new Dog(GetPetSkin("dog.png")),
            _ => null
        };
    }
}