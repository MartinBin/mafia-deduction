namespace Mafia_server;

public class General
{
    public static void StartServer()
    {
        InitServerData();
        ServerTCP.InitNetwork();
        Logger.Log(LogType.info2, "Server started");
    }
    private static void InitServerData()
    {
        for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
        {
            Globals.clients.Add(i, new Client());
        }
    }
}