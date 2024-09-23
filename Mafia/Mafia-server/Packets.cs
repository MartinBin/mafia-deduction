namespace Mafia_server;

public enum ServerPackets
{
    // Sent from server to client
    welcome = 1
}
public enum ClientPackets
{
    // Sent from client to server
    welcomeReceived = 1,
}