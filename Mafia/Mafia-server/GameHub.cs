using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Mafia_server.Decorator;

namespace Mafia_server
{
    public class GameHub : Hub
    {
        private readonly GameManager gameManager;
        private static bool gameInProgress = false;

        public GameHub(GameManager gameManager)
        {
            this.gameManager = gameManager;
            Logger.getInstance.SetLogFolderPath(".\\Logs");
        }


        public async Task StartGame()
        {
            if (Globals.clients.Count < Constants.MIN_PLAYERS_TO_START)
            {
                await Clients.Caller.SendAsync("Error", "Not enough players to start the game");
                return;
            }

            var earliestPlayer = Globals.clients.Values.OrderBy(c => c.JoinedAt).First();
            if (Context.ConnectionId != earliestPlayer.ConnectionId)
            {
                await Clients.Caller.SendAsync("Error", "Only the earliest joined player can start the game");
                return;
            }
            gameInProgress=true;
            List<Player> players = Globals.clients.Values.Select(c => c.Player).ToList();
            gameManager.AssignCharactersToPlayers(players);
            
            string clientImagePath = "path/to/image.png";
            string clientLabel = "VIP";
            string clientInfo = "Important character information.";
            
            foreach (var player in players)
            {
                IDecorator character = player.Character;
        
                IDecorator decoratedCharacter = new ImageDecorator(
                    new LabelDecorator(
                        new InfoDecorator(character, clientInfo), 
                        clientLabel), 
                    clientImagePath);

                decoratedCharacter.Render();
            }
            // Notify players of their assigned characters
            foreach (var client in Globals.clients.Values)
            {
                await Clients.Client(client.ConnectionId).SendAsync("AssignedCharacter", client.Player.Character.GetType().Name);
                await Clients.Client(client.ConnectionId).SendAsync("CanUseEvilChat", client.Player.Character.CanUseEvilChat);
            }
            await Clients.All.SendAsync("GameStarted");
            await PlayerList();
            Logger.getInstance.Log(LogType.Info, $"Game started");
        }

        public async Task PlayerList()
        {
            var players = Globals.clients.Values.Select(c => new { c.PlayerID, c.Username }).ToList();
            await Clients.All.SendAsync("SendPlayerList", players);
        }

        public async Task JoinGame(string username)
        {
            // Assign a player ID
            int playerId = Globals.AssignPlayerId();
            
            if (playerId != -1)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Players");
                Globals.clients[playerId] = new Client(playerId, username, Context.ConnectionId);
                
                if(gameInProgress){
                    await Clients.Caller.SendAsync("GameInProgress", playerId);
                }
                else
                {
                    await Clients.Caller.SendAsync("GameJoined", playerId);
                    await Clients.Caller.SendAsync("Welcome", playerId, "Welcome to the server!");
                    await Clients.Others.SendAsync("PlayerJoined", playerId, username);
                    await UpdatePlayerList();
                    Logger.getInstance.Log(LogType.Info, $"Player {username} (ID: {playerId}) joined the game.");
                    string joinMessage = $"{username} has joined the game!";
                    await SendMessage("Server", joinMessage);
                    await CheckAndNotifyGameReady();
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Server is full");
                Logger.getInstance.Log(LogType.Warning, "Server full");
            }
        }
        
        private async Task CheckAndNotifyGameReady()
        {
            if (Globals.clients.Count >= Constants.MIN_PLAYERS_TO_START)
            {
                var earliestPlayer = Globals.clients.Values.OrderBy(c => c.JoinedAt).First();
                await Clients.Client(earliestPlayer.ConnectionId).SendAsync("CanStartGame", true);
            }
            else
            {
                var earliestPlayer = Globals.clients.Values.OrderBy(c => c.JoinedAt).First();
                await Clients.Client(earliestPlayer.ConnectionId).SendAsync("CanStartGame", false);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var client = Globals.clients.Values.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (client != null)
            {
                Globals.clients.Remove(client.PlayerID);
                await Clients.Others.SendAsync("PlayerLeft", client.PlayerID, client.Username);
                Logger.getInstance.Log(LogType.Info, $"Player {client.Username} (ID: {client.PlayerID}) disconnected");
                string leaveMessage = $"{client.Username} has left the game!";
                await SendMessage("Server", leaveMessage);
                if (gameInProgress)
                {
                    await PlayerList();
                }
                else
                {
                    await UpdatePlayerList();
                    await CheckAndNotifyGameReady();
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        private async Task UpdatePlayerList()
        {
            var players = Globals.clients.Values.Select(c => new { c.PlayerID, c.Username }).ToList();
            await Clients.All.SendAsync("PlayerList", players);
        }
        // Add other game-related methods here
        
        public async Task SendMessage(string username, string message)
        {
            string fullMessage = $"{username}: {message}";
            Logger.getInstance.Log(LogType.Info, fullMessage);
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }
        
        
        public async Task SendGeneralMessage(int id,string message)
        {
            string fullMessage = $"{id}: {message}";
            Logger.getInstance.Log(LogType.Info, fullMessage);
            await Clients.All.SendAsync("ReceiveGeneralMessage",id, message);
        }

        public async Task SendEvilMessage(int id,string message)
        {
            string fullMessage = $"{id}: {message}";
            Logger.getInstance.Log(LogType.Info, fullMessage);
            var players = Globals.clients.Values.Where(x=>x.Player.Character.CanUseEvilChat).ToList();
            foreach (var player in players)
            {
                await Clients.Client(player.ConnectionId).SendAsync("ReceiveEvilMessage",id, message);
            }
        }
    }
}