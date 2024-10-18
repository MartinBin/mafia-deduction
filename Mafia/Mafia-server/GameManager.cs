namespace Mafia_server;
public class GameManager
{
    private List<CharacterFactory> factories;

    public GameManager()
    {
        factories = new List<CharacterFactory>
        {
            new GoodFactory(),
            new EvilFactory(),
            new NeutralFactory()
        };
    }

    public void AssignCharactersToPlayers(List<Player> players)
    {
        Random random = new Random();
        foreach (var player in players)
        {
            int factoryIndex = random.Next(factories.Count);
            Character character = factories[factoryIndex].CreateCharacter();
            player.AssignCharacter(character);
        }
    }
}