using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR.Client;

namespace Mafia_client;

public partial class LobbyControl : UserControl
{
    private readonly HubConnection hubConnection;
    private string playerName;
    private int playerId;
    public ObservableCollection<string> Players { get; private set; }

    public event EventHandler LeaveGameClicked;

    public LobbyControl(HubConnection connection, string playerName, int playerId)
    {
        InitializeComponent();
        hubConnection = connection;
        Players = new ObservableCollection<string>();
        PlayerListBox.ItemsSource = Players;
        this.playerName = playerName;
        this.playerId = playerId;
        SetupSignalREventHandlers();
    }

    private void LeaveButton_Click(object sender, RoutedEventArgs e)
    {
        LeaveGameClicked?.Invoke(this, EventArgs.Empty);
    }

    public void UpdatePlayerList(List<string> newPlayerList)
    {
        Dispatcher.Invoke(() =>
        {
            // Add missing players
            foreach (var player in newPlayerList.Except(Players))
            {
                Players.Add(player);
            }

            // Remove players that are no longer in the new list
            foreach (var player in Players.Except(newPlayerList).ToList())
            {
                Players.Remove(player);
            }

            UpdatePlayerCount(newPlayerList.Count);
        });
    }
    
    private void SetupSignalREventHandlers()
    {
        // Listen for chat messages
        hubConnection.On<string, string>("ReceiveMessage", (username, message) =>
        {
            Dispatcher.Invoke(() =>
            {
                string fullMessage = $"{username}: {message}";
                ChatMessagesListBox.Items.Add(fullMessage); // Add message to chat box
                ChatMessagesListBox.ScrollIntoView(ChatMessagesListBox.Items[ChatMessagesListBox.Items.Count - 1]);
            });
        });
        
        hubConnection.On<bool>("CanStartGame", (canStart) =>
        {
            Dispatcher.Invoke(() =>
            {
                StartButton.Visibility = canStart ? Visibility.Visible : Visibility.Collapsed;
            });
        });
        
        hubConnection.On<string>("Error", (errorMessage) =>
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        });

        hubConnection.On<string>("AssignedCharacter", (character) =>
        {
            Dispatcher.Invoke(() =>
            {
                // Store the assigned character for later use
                ((MainWindow)Application.Current.MainWindow).AssignedCharacter = character;
            });
        });

        hubConnection.On("GameStarted", () =>
        {
            Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).TransitionToGameWindow(hubConnection);
            });
        });
    }
    
    private void UpdatePlayerCount(int playerCount)
    {
        PlayerCountTextBlock.Text = playerCount.ToString();
    }
    
    private async void SendChatMessage_Click(object sender, RoutedEventArgs e)
    {
        string message = ChatInputTextBox.Text;

        if (!string.IsNullOrWhiteSpace(message))
        {
            await hubConnection.InvokeAsync("SendMessage",playerName, message);  // Send chat message
            ChatInputTextBox.Clear(); // Clear input box after sending
        }
    }


    private async void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await hubConnection.InvokeAsync("StartGame");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
