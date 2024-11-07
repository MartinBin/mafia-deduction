namespace Mafia_client.Factory;

public static class PetFactory
{
    public static IPet? CreatePet(string petType)
    {
        return petType.ToLower() switch
        {
            "cat" => new Cat(),
            "dog" => new Dog(),
            _ => null
        };
    }
}