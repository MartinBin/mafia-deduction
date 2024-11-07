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
        HashSet<int> assignedPlayerIds = new HashSet<int>();

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
        AssignSpecificCharacterToPlayers(players, goodGoonCount, new GoodFactory(), "GoodGoon", assignedPlayerIds);
        AssignSpecificCharacterToPlayers(players, goodSpyCount, new GoodFactory(), "GoodSpy", assignedPlayerIds);
        AssignSpecificCharacterToPlayers(players, medicCount, new GoodFactory(), "Medic", assignedPlayerIds);
        AssignSpecificCharacterToPlayers(players, evilGoonCount, new EvilFactory(), "EvilGoon", assignedPlayerIds);

        // Now assign the remaining characters randomly from the available factories
        AssignRemainingCharacters(players, remainingGoodCount, goodFactories, assignedPlayerIds);
        AssignRemainingCharacters(players, remainingEvilCount, evilFactories, assignedPlayerIds);
        AssignRemainingCharacters(players, remainingNeutralCount, neutralFactories, assignedPlayerIds);
    }

    private void AssignSpecificCharacterToPlayers(List<Player> players, int count, CharacterFactory factory, string roleName, HashSet<int> assignedPlayerIds)
    {
        var unassignedPlayers = players.Where(p => !assignedPlayerIds.Contains(p.ID)).ToList();
        Random random = new Random();

        for (int i = 0; i < count && unassignedPlayers.Count > 0; i++)
        {
            int index = random.Next(unassignedPlayers.Count);
            Player player = unassignedPlayers[index];
            Character character = factory.CreateCharacter();

            // Ensure the character is of the right type
            while (character.GetType().Name != roleName)
            {
                character = factory.CreateCharacter(); // Recreate if it's not the right role
            }

            player.AssignCharacter(character);
            assignedPlayerIds.Add(player.ID);
            unassignedPlayers.RemoveAt(index);
        }
    }

    private void AssignRemainingCharacters(List<Player> players, int count, List<CharacterFactory> factories, HashSet<int> assignedPlayerIds)
    {
        var unassignedPlayers = players.Where(p => !assignedPlayerIds.Contains(p.ID)).ToList();
        Random random = new Random();

        for (int i = 0; i < count && unassignedPlayers.Count > 0; i++)
        {
            int factoryIndex = random.Next(factories.Count);
            Character character = factories[factoryIndex].CreateCharacter();
            int playerIndex = random.Next(unassignedPlayers.Count);

            Player player = unassignedPlayers[playerIndex];
            player.AssignCharacter(character);
            assignedPlayerIds.Add(player.ID);
            unassignedPlayers.RemoveAt(playerIndex);
        }
    }
}