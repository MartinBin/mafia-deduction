using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Mafia_server.Command;
using Mafia_server.Decorator;
using Mafia_server.Observer;
using Mafia_server.Interpreter;
using Mafia_server.Log;

namespace Mafia_server
{
    public class GameHub : Hub, IObserver
    {
        private readonly GameManager gameManager;
        private readonly CommandInvoker _commandInvoker = new CommandInvoker();
        private readonly CommandInterpreter _commandInterpreter;
        private static bool gameInProgress = false;
        private static Logger logger = Logger.getInstance;

        private static Timer _stateTimer;
        private static int _currentTimeRemaining;
        private IHubContext<GameHub> _hubContext;
        public GameStateContext StateContext { get; private set; }

        public GameHub(GameManager gameManager, CommandSubject commandSubject, IHubContext<GameHub> hubContext)
        {
            StateContext = new GameStateContext(this);
            this.gameManager = gameManager;
            _hubContext = hubContext;

            // Register commands
            _commandInvoker.RegisterCommand("start", _ => new StartGameCommand(this));
            _commandInvoker.RegisterCommand("send", parameters => new SendMessageCommand(this, "Server", string.Join(" ", parameters)));

            // Initialize CommandInterpreter
            _commandInterpreter = new CommandInterpreter(_commandInvoker);

            commandSubject.RegisterObserver(this);

            // Set up logger
            var consoleHandler = new ConsoleLoggerHandler();
            var fileHandler = new FileLoggerHandler(AppDomain.CurrentDomain.BaseDirectory);
            consoleHandler.SetNext(fileHandler);
            logger.SetHandlerChain(consoleHandler);
        }

        public async Task BroadcastTimeRemaining(int timeMs)
        {
            await _hubContext.Clients.All.SendAsync("TimeRemaining", timeMs);
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
            gameInProgress = true;
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
            foreach (var client in Globals.clients.Values)
            {
                await Clients.Client(client.ConnectionId).SendAsync("AssignedCharacter", client.Player.Character.GetType().Name);
                await Clients.Client(client.ConnectionId).SendAsync("CanUseEvilChat", client.Player.Character.CanUseEvilChat);
            }

            await Task.Delay(100);

            await Clients.All.SendAsync("GameStarted");
            await PlayerList();
            logger.Log(LogType.Info, $"Game started");
            await Task.Delay(100);
            await StateContext.TransitionTo(new DayState(this, _hubContext, StateContext));
        }

        public async Task Vote(int targetId)
        {
            var currentState = StateContext.GetCurrentState();
            if (currentState is VotingState)
            {
                StateContext.HandlePlayerAction(GetPlayerId(), "Vote", targetId);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Voting is not allowed during this phase");
            }
        }

        public async Task UseAbility(string abilityName, int targetId)
        {
            var currentState = StateContext.GetCurrentState();
            if (currentState is NightState)
            {
                StateContext.HandlePlayerAction(GetPlayerId(), "UseAbility", abilityName, targetId);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Abilities can only be used during the night phase");
            }
        }

        private int GetPlayerId()
        {
            return Globals.clients.Values.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId)?.PlayerID ?? -1;
        }

        public async Task PlayerList()
        {
            var players = Globals.clients.Values.Select(c => new { c.PlayerID, c.Username }).ToList();
            await Clients.All.SendAsync("SendPlayerList", players);
        }

        public async Task JoinGame(string username)
        {
            int playerId = Globals.AssignPlayerId();

            if (playerId != -1)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Players");
                Globals.clients[playerId] = new Client(playerId, username, Context.ConnectionId);

                if (gameInProgress)
                {
                    await Clients.Caller.SendAsync("GameInProgress", playerId);
                }
                else
                {
                    await Clients.Caller.SendAsync("GameJoined", playerId);
                    await Clients.Caller.SendAsync("Welcome", playerId, "Welcome to the server!");
                    await Clients.Others.SendAsync("PlayerJoined", playerId, username);
                    await UpdatePlayerList();
                    logger.Log(LogType.Info, $"Player {username} (ID: {playerId}) joined the game.");
                    string joinMessage = $"{username} has joined the game!";
                    await SendMessage("Server", joinMessage);
                    await CheckAndNotifyGameReady();
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Server is full");
                logger.Log(LogType.Warning, "Server full");
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
                logger.Log(LogType.Info, $"Player {client.Username} (ID: {client.PlayerID}) disconnected");
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

        public async Task SendMessage(string username, string message)
        {
            string fullMessage = $"{username}: {message}";
            logger.Log(LogType.Info, fullMessage);
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }

        public async Task SendGeneralMessage(int id, string message)
        {
            string fullMessage = $"{id}: {message}";
            logger.Log(LogType.Info, fullMessage);
            await Clients.All.SendAsync("ReceiveGeneralMessage", id, message);
        }

        public async Task SendEvilMessage(int id, string message)
        {
            string fullMessage = $"{id}: {message}";
            logger.Log(LogType.Info, fullMessage);
            var players = Globals.clients.Values.Where(x => x.Player.Character.CanUseEvilChat).ToList();
            foreach (var player in players)
            {
                await Clients.Client(player.ConnectionId).SendAsync("ReceiveEvilMessage", id, message);
            }
        }

        public async Task UpdateAsync(string command, params string[] parameters)
        {
            await ExecuteCommand(command, parameters);
        }

        public async Task ExecuteCommand(string commandName, params string[] parameters)
        {
            await _commandInvoker.ExecuteCommandAsync(commandName, parameters);
        }

        public async Task InterpretCommand(string command)
        {
            await _commandInterpreter.InterpretAsync(command);
        }
    }
}
