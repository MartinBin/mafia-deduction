using System;
using System.Text.Json;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Windows.Threading;

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
        private int playerID;
        public Boolean playerCanSeeEvilChat { get; set; }
        private MainMenuControl mainMenuControl;
        private LobbyControl lobbyControl;
        private GameWindow gameWindow;
        public string AssignedCharacter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            mainMenuControl = new MainMenuControl();
            //lobbyControl = new LobbyControl(hubConnection);
            
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
            }
            catch (Exception ex)
            {
                StatusText.Text = "Connection failed";
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private async Task LeaveLobby()
        {
            if (hubConnection != null && hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await hubConnection.StopAsync();
                    StatusText.Text = "Left the lobby";
                    
                    // Switch back to main menu
                    MainContent.Content = mainMenuControl;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error leaving lobby: {ex.Message}");
                }
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

            hubConnection.Closed += (error) =>
            {
                Dispatcher.Invoke(() =>
                {
                    StatusText.Text = "Disconnected from server";
                    MainContent.Content = mainMenuControl;
                });
                return Task.CompletedTask;
            };
            hubConnection.On<int>("GameJoined", (id) =>
            {
                Dispatcher.Invoke(() =>
                {
                    playerID = id;
                    lobbyControl = new LobbyControl(hubConnection, playerName,playerID);
                    lobbyControl.LeaveGameClicked += LobbyControl_LeaveGameClicked;
                    MainContent.Content = lobbyControl;
                });
            });
            hubConnection.On<int,Boolean>("GameInProgress", (id,canSee) =>
            {
                Dispatcher.Invoke(() =>
                {   
                    playerID = id;
                    Boolean playerCanSeeEvilChat = canSee;
                    gameWindow = new GameWindow(hubConnection, AssignedCharacter,playerID,playerCanSeeEvilChat);
                    MainContent.Content = gameWindow;
                });
            });
        }

        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (hubConnection != null && hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.StopAsync();
            }
            base.OnClosing(e);
        }

        private async void LobbyControl_LeaveGameClicked(object sender, EventArgs e)
        {
            await LeaveLobby();
        }

        public void TransitionToGameWindow(HubConnection hubC)
        {
            if (string.IsNullOrEmpty(AssignedCharacter))
            {
                MessageBox.Show("Character not assigned yet!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
    
            gameWindow = new GameWindow(hubC, AssignedCharacter, playerID, playerCanSeeEvilChat);
            MainContent.Content = gameWindow;
        }
    }
}
