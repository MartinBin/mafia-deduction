namespace Mafia_server
{
    public class Client
    {
        public int PlayerID { get; private set; }
        public string Username { get; private set; }
        public string ConnectionId { get; private set; }
        public bool IsPlaying { get; set; } = false;
        public Player Player { get; private set; }

        public Client(int playerId, string username, string connectionId)
        {
            PlayerID = playerId;
            Username = username;
            ConnectionId = connectionId;
            Player = new Player(playerId);
        }
    }
}