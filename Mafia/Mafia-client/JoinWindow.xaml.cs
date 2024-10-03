using System.Windows;

namespace Mafia_client
{
    public partial class JoinWindow : Window
    {
        public string ServerIp { get; private set; }
        public string PlayerName { get; private set; }

        public JoinWindow()
        {
            InitializeComponent();
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            ServerIp = ServerIpTextBox.Text.Trim();
            PlayerName = PlayerNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(ServerIp) || string.IsNullOrEmpty(PlayerName))
            {
                MessageBox.Show("Please enter both server IP and your name.");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}