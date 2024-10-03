using Microsoft.AspNetCore.SignalR;

namespace Mafia_server
{
    public class ServerSend
    {
        private readonly IHubContext<GameHub> _hubContext;

        public ServerSend(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToPlayer(int playerId, string method, params object[] args)
        {
            if (Globals.clients.TryGetValue(playerId, out var client))
            {
                await _hubContext.Clients.Client(client.ConnectionId).SendAsync(method, args);
            }
        }

        public async Task SendToAllPlayers(string method, params object[] args)
        {
            await _hubContext.Clients.Group("Players").SendAsync(method, args);
        }

        // Add other methods for specific game actions
    }
}