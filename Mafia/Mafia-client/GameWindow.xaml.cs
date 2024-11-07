using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Mafia_client.Prototype;
using Microsoft.AspNetCore.SignalR.Client;

namespace Mafia_client;


public partial class GameWindow : UserControl, INotifyPropertyChanged, IDisposable
{
    private const int MaxPlayers = 10;
    private const double TableCenterX = 400;
    private const double TableCenterY = 300;
    private const double TableRadius = 200;
    private readonly HubConnection hubConnection;
    private readonly string assignedCharacter;
    private int playerID;

    public GameWindow(HubConnection connection, string character, int playerID, Boolean playerCanSeeEvilChat)
    {
        InitializeComponent();
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
    private async void RequestPlayerList()
    {
        await hubConnection.InvokeAsync("PlayerList");
    }

    public void SetupPlayers(List<Player> playerNames)
    {
        GameCanvas.Children.Clear();
        GameCanvas.Children.Add(TableEllipse);

        int playerCount = Math.Min(playerNames.Count, MaxPlayers);
        double angleStep = 2 * Math.PI / playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            double angle = i * angleStep;
            double x = TableCenterX + TableRadius * Math.Cos(angle);
            double y = TableCenterY + TableRadius * Math.Sin(angle);
            
            AddPlayerToCanvas(playerNames[i].username, x / GameCanvas.Width, y / GameCanvas.Height,playerNames[i].playerID);
        }
    }
    
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

    private void AddPlayerToCanvas(string playerName, double relativeX, double relativeY, int playerID)
    {
        Ellipse playerAvatar = playerAvatarPrototype.Clone();
        TextBlock nameLabel = nameLabelPrototype.Clone();
        nameLabel.Text = playerName;
        
        Canvas.SetLeft(playerAvatar, relativeX * GameCanvas.Width - 25);
        Canvas.SetTop(playerAvatar, relativeY * GameCanvas.Height - 25);
        Canvas.SetLeft(nameLabel, relativeX * GameCanvas.Width - 50);
        Canvas.SetTop(nameLabel, relativeY * GameCanvas.Height - 60);

        GameCanvas.Children.Add(playerAvatar);
        GameCanvas.Children.Add(nameLabel);
        if (this.playerID == playerID)
        {
            TextBlock roleLabel = new TextBlock
            {
                Text = assignedCharacter,
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Width = 100
            };
            Canvas.SetLeft(roleLabel, relativeX * GameCanvas.Width - 50);
            Canvas.SetTop(roleLabel, relativeY * GameCanvas.Height - 80);
            GameCanvas.Children.Add(roleLabel);
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
            });
        });

        hubConnection.On<int,string>("ReceiveEvilMessage", (id,message) =>
        {
            Dispatcher.Invoke(() =>
            {
                string fullMessage = $"{id}: {message}";
                EvilMessageList.Items.Add(fullMessage);
            });
        });
    }
    
    
    private async void SendGeneralMessage(object sender, RoutedEventArgs e)
    {
        string message = GeneralMessageInput.Text;
        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                await hubConnection.InvokeAsync("SendGeneralMessage",playerID, message);
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
                await hubConnection.InvokeAsync("SendEvilMessage",playerID, message);
                EvilMessageInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        hubConnection?.DisposeAsync();
    }
}
