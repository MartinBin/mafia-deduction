namespace Mafia_server;

public static class Globals
{
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public static bool serverIsRunning = false;

    public static int AssignPlayerId()
    {
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            if (!clients.ContainsKey(i))
            {
                return i;
            }
        }
        return -1; // Server is full
    }
}