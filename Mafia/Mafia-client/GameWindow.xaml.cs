using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;

namespace Mafia_client;


public partial class GameWindow : UserControl
{
    private const int MaxPlayers = 10;
    private const double TableCenterX = 400;
    private const double TableCenterY = 300;
    private const double TableRadius = 200;
    private readonly HubConnection hubConnection;
    private readonly string assignedCharacter;
    private int playerID;

    public GameWindow(HubConnection connection, string character, int playerID)
    {
        InitializeComponent();
        hubConnection = connection;
        assignedCharacter = character;
        this.playerID = playerID;
        if (hubConnection.State != HubConnectionState.Connected)
        {
            MessageBox.Show("Hub connection is not in a connected state.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        SetupSignalREventHandlers();
        RequestPlayerList();
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

    private void AddPlayerToCanvas(string playerName, double relativeX, double relativeY, int playerID)
    {
        Ellipse playerAvatar = new Ellipse
        {
            Width = 50,
            Height = 50,
            Fill = Brushes.LightBlue,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        TextBlock nameLabel = new TextBlock
        {
            Text = playerName,
            FontSize = 14,
            TextAlignment = TextAlignment.Center,
            Width = 100
        };
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
    }
}
