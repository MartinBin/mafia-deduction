using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Mafia_client.Factory;
using Mafia_client.Prototype;
using System.Windows.Media.Imaging;
using Mafia_client.Client;
using Mafia_client.Visitor;
using Microsoft.AspNetCore.SignalR.Client;

namespace Mafia_client;

public partial class GameWindow : UserControl, INotifyPropertyChanged, IDisposable
{
    private const int MaxPlayers = 10;
    private const double TableCenterX = 400;
    private const double TableCenterY = 300;
    private const double TableRadius = 200;
    private string currentGameState;
    private int timeRemaining;
    private readonly HubConnection hubConnection;
    private string assignedCharacter;
    private int playerID;
    private readonly bool playerCanSeeEvilChat;
    private readonly Dictionary<int, int> voteCount = new Dictionary<int, int>();
    
    private static readonly List<string> AvailablePetTypes = new List<string> { "cat", "dog" };
    private static readonly Random RandomGenerator = new Random();
    private readonly Dictionary<int, ClientPlayer> players = new Dictionary<int, ClientPlayer>();
    
    private readonly EllipsePrototype playerAvatarPrototype = new EllipsePrototype(
        width: 50,
        height: 50,
        fill: Brushes.LightBlue,
        stroke: Brushes.Black,
        strokeThickness: 2);

    private readonly TextBlockPrototype nameLabelPrototype = new TextBlockPrototype(
        text: String.Empty,
        fontSize: 14,
        textAlignment: TextAlignment.Center,
        width: 100);

    public GameWindow(HubConnection connection, string character, int playerID, Boolean playerCanSeeEvilChat)
    {
        InitializeComponent();
        DataContext = this;
        hubConnection = connection;
        assignedCharacter = character;
        this.playerID = playerID;
        IsPlayerEvil = playerCanSeeEvilChat;
        
        if (hubConnection.State != HubConnectionState.Connected)
        {
            MessageBox.Show("Hub connection is not in a connected state.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        SetupSignalREventHandlers();
        RequestPlayerList();
    }

    private bool _isPlayerEvil;
    public bool IsPlayerEvil
    {
        get => _isPlayerEvil;
        set
        {
            _isPlayerEvil = value;
            OnPropertyChanged(nameof(IsPlayerEvil));
        }
    }

    public string CurrentGameState
    {
        get => currentGameState;
        set
        {
            currentGameState = value;
            OnPropertyChanged(nameof(CurrentGameState));
        }
    }

    public int TimeRemaining
    {
        get => timeRemaining;
        set
        {
            timeRemaining = value;
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(TimeRemainingText));
        }
    }

    public string TimeRemainingText => $"Time Remaining: {TimeRemaining / 1000} seconds";
        private async void RequestPlayerList()
    {
        await hubConnection.InvokeAsync("PlayerList");
    }

    private void InitializePlayers(List<(int id, string name)> playerList)
    {
        players.Clear();
        foreach (var (id, name) in playerList)
        {
            players[id] = new ClientPlayer(id, name);
        }
        ArrangePlayers();
    }

    private void ArrangePlayers()
    {
        GameCanvas.Children.Clear();
        GameCanvas.Children.Add(TableEllipse);

        int playerCount = players.Count;
        double angleStep = 2 * Math.PI / playerCount;
        int currentIndex = 0;

        foreach (var player in players.Values)
        {
            double angle = currentIndex * angleStep;
            double x = TableCenterX + TableRadius * Math.Cos(angle);
            double y = TableCenterY + TableRadius * Math.Sin(angle);
            
            player.Position = new Point(x, y);
            var composite = CreatePlayerVisual(player);
            player.Visual = composite;
            composite.Render(GameCanvas, x, y);
            
            currentIndex++;
        }
    }

    private List<ClientPlayer> GetAlivePlayers()
    {
        return players.Values.Where(p => p.IsAlive).ToList();
    }

    public void SetupPlayers(List<Player> playerList)
    {
        GameCanvas.Children.Clear();
        GameCanvas.Children.Add(TableEllipse);
        players.Clear();

        int playerCount = Math.Min(playerList.Count, MaxPlayers);
        double angleStep = 2 * Math.PI / playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            var player = playerList[i];
            double angle = i * angleStep;
            double x = TableCenterX + TableRadius * Math.Cos(angle);
            double y = TableCenterY + TableRadius * Math.Sin(angle);
            
            var clientPlayer = new ClientPlayer(player.playerID, player.username)
            {
                Position = new Point(x, y)
            };
            players[player.playerID] = clientPlayer;
            
            var composite = CreatePlayerVisual(clientPlayer);
            clientPlayer.Visual = composite;
            composite.Render(GameCanvas, x, y);
        }
    }

    private PlayerComposite CreatePlayerVisual(ClientPlayer player)
    {
        var playerComposite = new PlayerComposite();

        // Add avatar
        playerComposite.AddComponent(new PlayerAvatar(playerAvatarPrototype));

        // Add name label
        playerComposite.AddComponent(new PlayerLabel(nameLabelPrototype, player.Name));
        
        // Add role label if it's the current player
        if (player.Id == playerID)
        {
            var roleLabel = new PlayerRoleLabel(
                new TextBlockPrototype(assignedCharacter, 14, TextAlignment.Center, 100),
                assignedCharacter);
            playerComposite.AddComponent(roleLabel);
            
            string petType = GetRandomPetType();
            playerComposite.AddComponent(new PlayerPet(petType));
        }

        return playerComposite;
    }

    private string GetRandomPetType()
    {
        int index = RandomGenerator.Next(AvailablePetTypes.Count);
        return AvailablePetTypes[index];
    }

    private void UpdatePlayerVisual(ClientPlayer player)
    {
        if (!player.IsAlive)
        {
            var deathVisitor = new DeathEffectVisitor();
            player.Visual.Accept(deathVisitor);
        }
    }
        private void SetupSignalREventHandlers()
    {
        hubConnection.On<JsonElement>("SendPlayerList", (jsonElement) =>
        {
            List<Player> playerList = JsonSerializer.Deserialize<List<Player>>(jsonElement.GetRawText());
            Dispatcher.Invoke(() =>
            {
                SetupPlayers(playerList.ToList());
            });
        });

        hubConnection.On<int,string>("ReceiveGeneralMessage", (id,message) =>
        {
            Dispatcher.Invoke(() =>
            {
                string fullMessage = $"{id}: {message}";
                GeneralMessageList.Items.Add(fullMessage);
                GeneralMessageList.ScrollIntoView(GeneralMessageList.Items[GeneralMessageList.Items.Count - 1]);
            });
        });

        hubConnection.On<int,string>("ReceiveEvilMessage", (id,message) =>
        {
            Dispatcher.Invoke(() =>
            {
                string fullMessage = $"{id}: {message}";
                EvilMessageList.Items.Add(fullMessage);
                EvilMessageList.ScrollIntoView(EvilMessageList.Items[EvilMessageList.Items.Count - 1]);
            });
        });

        hubConnection.On<string>("StateChanged", (state) =>
        {
            Dispatcher.Invoke(() =>
            {
                CurrentGameState = state;
                UpdateUIForGameState(state);
            });
        });

        hubConnection.On<int>("TimeRemaining", (timeMs) =>
        {
            Dispatcher.Invoke(() =>
            {
                TimeRemaining = timeMs;
            });
        });

        hubConnection.On<string>("GameResults", (winningTeam) =>
        {
            Dispatcher.Invoke(() =>
            {
                ShowGameResults(winningTeam);
            });
        });

        hubConnection.On<int, int>("VoteRegistered", (voterId, targetId) =>
        {
            Dispatcher.Invoke(() =>
            {
                UpdateVoteUI(voterId, targetId);
            });
        });

        hubConnection.On<int>("PlayerDied", (playerId) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (players.TryGetValue(playerId, out var player))
                {
                    player.IsAlive = false;
                    UpdatePlayerVisual(player);
                }
            });
        });

        hubConnection.On<List<(int id, string name)>>("UpdatePlayerList", (playerList) =>
        {
            Dispatcher.Invoke(() =>
            {
                InitializePlayers(playerList);
            });
        });
        
        hubConnection.On("GameStarted", () =>
        {
            Dispatcher.Invoke(() =>
            {
                ClearGameState();
                MainGamePanel.Visibility = Visibility.Visible;
                GameEndPanel.Visibility = Visibility.Collapsed;
            
                // Enable appropriate controls for initial game state
                UpdateUIForGameState("Day");
            });
        });

        hubConnection.On<dynamic>("AssignedCharacter", (characterInfo) =>
        {
            Dispatcher.Invoke(() =>
            {
                assignedCharacter = characterInfo.Type;
                IsPlayerEvil = characterInfo.CanUseEvilChat;
                playerID = characterInfo.PlayerId;
            
                // Update UI based on character assignment
                UpdateCharacterDisplay();
            });
        });
    }
        
    private void UpdateCharacterDisplay()
    {
        // Update any UI elements that show the player's character
        if (players.TryGetValue(playerID, out var currentPlayer))
        {
            var roleLabel = new PlayerRoleLabel(
                new TextBlockPrototype(assignedCharacter, 14, TextAlignment.Center, 100),
                assignedCharacter);
        
            currentPlayer.Visual?.AddComponent(roleLabel);
        }
    }

    private void UpdateUIForGameState(string state)
    {
        switch (state)
        {
            case "Day":
                EnableDayControls();
                break;
            case "Night":
                EnableNightControls();
                break;
            case "Voting":
                EnableVotingControls();
                break;
            case "GameEnd":
                EnableEndGameControls();
                break;
        }
    }

    private void EnableDayControls()
    {
        DayPanel.Visibility = Visibility.Visible;
        NightPanel.Visibility = Visibility.Collapsed;
        VotingPanel.Visibility = Visibility.Collapsed;
        
        ChatTabControl.SelectedIndex = 0;
        GeneralMessageInput.IsEnabled = true;
        EvilMessageInput.IsEnabled = IsPlayerEvil;
    }

    private void EnableNightControls()
    {
        DayPanel.Visibility = Visibility.Collapsed;
        NightPanel.Visibility = Visibility.Visible;
        VotingPanel.Visibility = Visibility.Collapsed;

        SetupNightAbilities();
        
        GeneralMessageInput.IsEnabled = false;
        EvilMessageInput.IsEnabled = IsPlayerEvil;
    }

    private void EnableVotingControls()
    {
        DayPanel.Visibility = Visibility.Collapsed;
        NightPanel.Visibility = Visibility.Collapsed;
        VotingPanel.Visibility = Visibility.Visible;

        SetupVotingButtons();
        
        GeneralMessageInput.IsEnabled = true;
        EvilMessageInput.IsEnabled = IsPlayerEvil;
    }

    private void EnableEndGameControls()
    {
        GameEndPanel.Visibility = Visibility.Visible;
        MainGamePanel.Visibility = Visibility.Collapsed;
        
        GeneralMessageInput.IsEnabled = false;
        EvilMessageInput.IsEnabled = false;
    }
        private void SetupNightAbilities()
    {
        AbilitiesPanel.Children.Clear();
        
        switch (assignedCharacter)
        {
            case "EvilGoon":
                AddAbilityButton("Kill", "Select a player to kill");
                break;
            case "Medic":
                AddAbilityButton("Heal", "Select a player to heal");
                break;
            case "GoodSpy":
            case "EvilSpy":
                AddAbilityButton("Investigate", "Select a player to investigate");
                break;
        }
    }

    private void AddAbilityButton(string abilityName, string tooltip)
    {
        var button = new Button
        {
            Content = abilityName,
            ToolTip = tooltip,
            Margin = new Thickness(5),
            Padding = new Thickness(10.0, 5.0, 10.0, 10.0),
            Background = new SolidColorBrush(Colors.DarkBlue),
            Foreground = new SolidColorBrush(Colors.White)
        };
        button.Click += async (s, e) => await OnAbilityButtonClick(abilityName);
        AbilitiesPanel.Children.Add(button);
    }

    private async Task OnAbilityButtonClick(string abilityName)
    {
        EnablePlayerSelection(async (targetId) =>
        {
            try
            {
                await hubConnection.InvokeAsync("UseAbility", abilityName, targetId);
                MessageBox.Show($"{abilityName} ability used on player {targetId}", "Ability Used", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error using ability: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DisablePlayerSelection();
            }
        });
    }

    private void EnablePlayerSelection(Action<int> onPlayerSelected)
    {
        foreach (var player in GetAlivePlayers())
        {
            if (player.Id == playerID) continue; // Skip self

            if (player.Visual?.GetComponent<PlayerAvatar>()?.element is Ellipse avatarEllipse)
            {
                // Store original state
                var originalBrush = avatarEllipse.Fill;
                var originalCursor = avatarEllipse.Cursor;

                // Highlight selectable players
                avatarEllipse.Fill = new SolidColorBrush(Colors.Yellow);
                avatarEllipse.Cursor = Cursors.Hand;
                
                void PlayerSelected(object sender, MouseButtonEventArgs e)
                {
                    // Reset all highlights
                    DisablePlayerSelection();
                    
                    // Invoke callback with selected player ID
                    onPlayerSelected(player.Id);
                    
                    // Remove this event handler
                    avatarEllipse.MouseDown -= PlayerSelected;
                }

                avatarEllipse.MouseDown += PlayerSelected;
            }
        }
    }

    private void DisablePlayerSelection()
    {
        foreach (var player in players.Values)
        {
            if (player.Visual?.GetComponent<PlayerAvatar>()?.element is Ellipse avatarEllipse)
            {
                avatarEllipse.Fill = player.IsAlive ? 
                    new SolidColorBrush(Colors.LightBlue) : 
                    new SolidColorBrush(Colors.Gray);
                avatarEllipse.Cursor = Cursors.Arrow;
                
                // Clear all mouse event handlers
                avatarEllipse.MouseDown -= null;
            }
        }
    }

    private void SetupVotingButtons()
    {
        VotingPanel.Children.Clear();
        voteCount.Clear();

        foreach (var player in GetAlivePlayers())
        {
            if (player.Id != playerID) // Can't vote for yourself
            {
                var button = new Button
                {
                    Content = $"Vote {player.Name}",
                    Tag = player.Id,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10.0, 5.0, 5.0, 10.0),
                    Background = new SolidColorBrush(Colors.DarkRed),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                
                button.Click += async (s, e) => 
                {
                    try
                    {
                        await hubConnection.InvokeAsync("Vote", (int)((Button)s).Tag);
                        // Disable all voting buttons after voting
                        foreach (Button voteButton in VotingPanel.Children)
                        {
                            voteButton.IsEnabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error voting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };
                
                VotingPanel.Children.Add(button);
            }
        }
    }

    private void UpdateVoteUI(int voterId, int targetId)
    {
        if (!voteCount.ContainsKey(targetId))
        {
            voteCount[targetId] = 0;
        }
        voteCount[targetId]++;

        if (players.TryGetValue(voterId, out var voter) && 
            players.TryGetValue(targetId, out var target))
        {
            // Add or update vote line
            var voteLine = new Line
            {
                X1 = voter.Position.X,
                Y1 = voter.Position.Y,
                X2 = target.Position.X,
                Y2 = target.Position.Y,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 2,
                Tag = $"vote_{voterId}"
            };

            // Remove existing vote line from this voter if it exists
            var existingLines = GameCanvas.Children.OfType<Line>()
                .Where(l => l.Tag?.ToString() == $"vote_{voterId}")
                .ToList();
            foreach (var line in existingLines)
            {
                GameCanvas.Children.Remove(line);
            }

            GameCanvas.Children.Add(voteLine);

            // Update or add vote count display
            var voteText = GameCanvas.Children.OfType<TextBlock>()
                .FirstOrDefault(t => t.Tag?.ToString() == $"voteCount_{targetId}");

            if (voteText == null)
            {
                voteText = new TextBlock
                {
                    Tag = $"voteCount_{targetId}",
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))
                };
                GameCanvas.Children.Add(voteText);
            }

            voteText.Text = $"Votes: {voteCount[targetId]}";
            Canvas.SetLeft(voteText, target.Position.X - 25);
            Canvas.SetTop(voteText, target.Position.Y + 30);
        }
    }
        private async void SendGeneralMessage(object sender, RoutedEventArgs e)
    {
        string message = GeneralMessageInput.Text;
        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                await hubConnection.InvokeAsync("SendGeneralMessage", playerID, message);
                GeneralMessageInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void SendEvilMessage(object sender, RoutedEventArgs e)
    {
        string message = EvilMessageInput.Text;
        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                await hubConnection.InvokeAsync("SendEvilMessage", playerID, message);
                EvilMessageInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ShowGameResults(string winningTeam)
    {
        GameResultText.Text = $"Game Over! {winningTeam} team wins!";
        
        var result = MessageBox.Show(
            $"Game Over! {winningTeam} team wins!\nReturn to lobby?",
            "Game Results",
            MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                hubConnection.InvokeAsync("ReturnToLobby");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error returning to lobby: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void ReturnToLobby_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await hubConnection.InvokeAsync("ReturnToLobby");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error returning to lobby: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ClearGameState()
    {
        // Clear all game-related collections and states
        players.Clear();
        voteCount.Clear();
        GameCanvas.Children.Clear();
        GameCanvas.Children.Add(TableEllipse);
        
        // Clear chat
        GeneralMessageList.Items.Clear();
        EvilMessageList.Items.Clear();
        
        // Reset input fields
        GeneralMessageInput.Clear();
        EvilMessageInput.Clear();
        
        // Reset panels
        DayPanel.Visibility = Visibility.Collapsed;
        NightPanel.Visibility = Visibility.Collapsed;
        VotingPanel.Visibility = Visibility.Collapsed;
        GameEndPanel.Visibility = Visibility.Collapsed;
        MainGamePanel.Visibility = Visibility.Visible;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        try
        {
            // Clear game state
            ClearGameState();
            
            // Remove all event handlers
            if (hubConnection != null)
            {
                hubConnection.Remove("SendPlayerList");
                hubConnection.Remove("ReceiveGeneralMessage");
                hubConnection.Remove("ReceiveEvilMessage");
                hubConnection.Remove("StateChanged");
                hubConnection.Remove("TimeRemaining");
                hubConnection.Remove("GameResults");
                hubConnection.Remove("VoteRegistered");
                hubConnection.Remove("PlayerDied");
                hubConnection.Remove("UpdatePlayerList");
            }

            // Dispose of the hub connection
            hubConnection?.DisposeAsync().AsTask().Wait();
        }
        catch (Exception ex)
        {
            // Log any disposal errors
            MessageBox.Show($"Error during cleanup: {ex.Message}", "Cleanup Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}