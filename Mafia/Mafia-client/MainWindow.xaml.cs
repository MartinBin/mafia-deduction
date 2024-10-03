using System;
using System.Text.Json;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Mafia_client
{
    public class Player
    {
        public int playerID { get; set; }
        public string username { get; set; }
    }
    public partial class MainWindow : Window
    {
        private HubConnection hubConnection;
        private string playerName;
        private MainMenuControl mainMenuControl;
        private LobbyControl lobbyControl;

        public MainWindow()
        {
            InitializeComponent();
            mainMenuControl = new MainMenuControl();
            lobbyControl = new LobbyControl();
            
            mainMenuControl.JoinGameClicked += MainMenuControl_JoinGameClicked;
            mainMenuControl.ExitGameClicked += MainMenuControl_ExitGameClicked;

            MainContent.Content = mainMenuControl;
        }

        private void MainMenuControl_JoinGameClicked(object sender, EventArgs e)
        {
            var joinWindow = new JoinWindow();
            if (joinWindow.ShowDialog() == true)
            {
                string serverIp = joinWindow.ServerIp;
                playerName = joinWindow.PlayerName;
                _ = ConnectToServer(serverIp);
            }
        }

        private void MainMenuControl_ExitGameClicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async Task ConnectToServer(string serverIp)
        {
            try
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl($"http://{serverIp}/gamehub")
                    .WithAutomaticReconnect()
                    .Build();

                SetupSignalREventHandlers();

                await hubConnection.StartAsync();
                StatusText.Text = "Connected to server";
                
                // Join the game
                await hubConnection.InvokeAsync("JoinGame", playerName);

                // Switch to lobby interface
                MainContent.Content = lobbyControl;
                //lobbyControl.UpdatePlayerList(new List<string> { playerName }); // Add the current player
            }
            catch (Exception ex)
            {
                StatusText.Text = "Connection failed";
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private void SetupSignalREventHandlers()
        {
            hubConnection.On<JsonElement>("PlayerList", (jsonElement) =>
            {
                var playerList = JsonSerializer.Deserialize<List<Player>>(jsonElement.GetRawText());
                
                Dispatcher.Invoke(() =>
                {
                    var players = playerList.Select(p => $"{p.username} (ID: {p.playerID})").ToList();
                    lobbyControl.UpdatePlayerList(players);
                    StatusText.Text = $"Players in lobby: {playerList.Count}";
                });
            });

            hubConnection.On("GameStarted", () =>
            {
                Dispatcher.Invoke(() =>
                {
                    StatusText.Text = "Game is starting...";
                    // TODO: Switch to game interface
                });
            });

            // Add more event handlers as needed
        }

        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (hubConnection != null && hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.StopAsync();
            }
            base.OnClosing(e);
        }
    }
}