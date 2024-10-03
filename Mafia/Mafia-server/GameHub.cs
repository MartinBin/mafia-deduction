using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Mafia_server
{
    public class GameHub : Hub
    {
        public async Task JoinGame(string username)
        {
            // Assign a player ID
            int playerId = Globals.AssignPlayerId();
            
            if (playerId != -1)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Players");
                Globals.clients[playerId] = new Client(playerId, username, Context.ConnectionId);
                
                await Clients.Caller.SendAsync("Welcome", playerId, "Welcome to the server!");
                await Clients.Others.SendAsync("PlayerJoined", playerId, username);
                await UpdatePlayerList();
                Logger.Log(LogType.Info, $"Player {username} (ID: {playerId}) joined the game.");
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Server is full");
                Logger.Log(LogType.Warning, "Server full");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var client = Globals.clients.Values.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (client != null)
            {
                Globals.clients.Remove(client.PlayerID);
                await Clients.Others.SendAsync("PlayerLeft", client.PlayerID, client.Username);
                Logger.Log(LogType.Info, $"Player {client.Username} (ID: {client.PlayerID}) disconnected");
                
                await UpdatePlayerList();
            }

            await base.OnDisconnectedAsync(exception);
        }
        private async Task UpdatePlayerList()
        {
            var players = Globals.clients.Values.Select(c => new { c.PlayerID, c.Username }).ToList();
            await Clients.All.SendAsync("PlayerList", players);
        }
        // Add other game-related methods here
    }
}