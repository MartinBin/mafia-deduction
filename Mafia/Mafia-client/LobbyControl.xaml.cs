using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Mafia_client;

public partial class LobbyControl : UserControl
{
    public ObservableCollection<string> Players { get; private set; }

    public LobbyControl()
    {
        InitializeComponent();
        Players = new ObservableCollection<string>();
        PlayerListBox.ItemsSource = Players;
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

    private void UpdatePlayerCount(int playerCount)
    {
        PlayerCountTextBlock.Text = playerCount.ToString();
    }
}