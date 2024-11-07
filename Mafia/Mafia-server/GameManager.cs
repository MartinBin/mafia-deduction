namespace Mafia_server;
public class GameManager
{
    private List<CharacterFactory> goodFactories;
    private List<CharacterFactory> evilFactories;
    private List<CharacterFactory> neutralFactories;

    public GameManager()
    {
        goodFactories = new List<CharacterFactory> { new GoodFactory() };
        evilFactories = new List<CharacterFactory> { new EvilFactory() };
        neutralFactories = new List<CharacterFactory> { new NeutralFactory() };
    }

    public void AssignCharactersToPlayers(List<Player> players)
    {
        Random random = new Random();
        int totalPlayers = players.Count;

        // Ensure the minimum required characters
        int goodGoonCount = 1;
        int goodSpyCount = 1;
        int medicCount = 1;
        int evilGoonCount = 1;

        // Calculate remaining characters
        int remainingGoodCount = (totalPlayers / 2) - (goodGoonCount + goodSpyCount + medicCount);
        int remainingEvilCount = (totalPlayers / 3) - evilGoonCount;
        int remainingNeutralCount = totalPlayers - (goodGoonCount + goodSpyCount + medicCount + evilGoonCount + remainingGoodCount + remainingEvilCount);

        // First, assign the guaranteed characters
        AssignSpecificCharacterToPlayers(players, goodGoonCount, new GoodFactory(), "GoodGoon");
        AssignSpecificCharacterToPlayers(players, goodSpyCount, new GoodFactory(), "GoodSpy");
        AssignSpecificCharacterToPlayers(players, medicCount, new GoodFactory(), "Medic");
        AssignSpecificCharacterToPlayers(players, evilGoonCount, new EvilFactory(), "EvilGoon");

        // Now assign the remaining characters randomly from the available factories
        AssignRemainingCharacters(players, remainingGoodCount, goodFactories);
        AssignRemainingCharacters(players, remainingEvilCount, evilFactories);
        AssignRemainingCharacters(players, remainingNeutralCount, neutralFactories);
    }

    private void AssignSpecificCharacterToPlayers(List<Player> players, int count, CharacterFactory factory, string roleName)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            var player = players[random.Next(players.Count)];
            Character character = factory.CreateCharacter();

            // Ensure the character is of the right type
            while (character.GetType().Name != roleName)
            {
                character = factory.CreateCharacter(); // Recreate if it's not the right role
            }

            player.AssignCharacter(character);
        }
    }

    private void AssignRemainingCharacters(List<Player> players, int count, List<CharacterFactory> factories)
    {
        Random random = new Random();
        for (int i = 0; i < count; i++)
        {
            int factoryIndex = random.Next(factories.Count);
            Character character = factories[factoryIndex].CreateCharacter();
            var player = players[random.Next(players.Count)];
            player.AssignCharacter(character);
        }
    }
}