using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR.Client;

namespace Mafia_client;

public partial class LobbyControl : UserControl
{
    private readonly HubConnection hubConnection;
    private string playerName;
    public ObservableCollection<string> Players { get; private set; }

    public LobbyControl(HubConnection connection, string playerName)
    {
        InitializeComponent();
        hubConnection = connection;
        Players = new ObservableCollection<string>();
        PlayerListBox.ItemsSource = Players;
        this.playerName = playerName;
        SetupSignalREventHandlers();
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


}