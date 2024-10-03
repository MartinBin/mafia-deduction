using System;
using System.Windows.Controls;

namespace Mafia_client
{
    public partial class MainMenuControl : UserControl
    {
        public event EventHandler JoinGameClicked;
        public event EventHandler ExitGameClicked;

        public MainMenuControl()
        {
            InitializeComponent();
        }

        private void JoinGame_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            JoinGameClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ExitGame_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ExitGameClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}