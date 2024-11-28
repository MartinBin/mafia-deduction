using System.Windows;

namespace Mafia_client.Client;

public class ClientPlayer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsAlive { get; set; }
    public Point Position { get; set; }
    public PlayerComposite Visual { get; set; }

    public ClientPlayer(int id, string name)
    {
        Id = id;
        Name = name;
        IsAlive = true;
    }
}