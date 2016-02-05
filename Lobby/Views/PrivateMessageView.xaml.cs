using Lobby.Model;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media;
using Lobby.DAL;

namespace Lobby.Views
{
    /// <summary>
    /// Logique d'interaction pour PrivateMessageView.xaml
    /// </summary>
    public partial class PrivateMessageView : Window
    {
        private string _receiverName = null;
        private LobbyView _attachedLobby = null;
        private MediaPlayer _mediaPlayer = new MediaPlayer();

        public string ChattingTo
        {
            get { return _receiverName; }
        }

        public PrivateMessageView(LobbyView lobby, string receiverName)
        {
            InitializeComponent();

            _receiverName = receiverName.Trim();
            Title = receiverName.Trim() + " (Privé) - MillesBornes";
            _attachedLobby = lobby;

            lblReceiverName.Text = receiverName.Trim();
            GetResponses();
        }

        /// <summary>
        /// Boucle qui s'assure de recevoir les réponses, et que 
        /// l'utilisateur soit encore connecté.
        /// </summary>
        private async void GetResponses()
        {
            while (await StillConnected())
            {
                // Récupère les messages.
                UpdateMessages(lvMessages);

                await Task.Delay(2000);
            }

            // N'est plus connecté.
            Close();
        }

        /// <summary>
        /// Détermine si l'utilisateur est encore connecté.
        /// </summary>
        /// <returns>True si encore connecté.</returns>
        private async Task<bool> StillConnected()
        {
            bool ok = await Task<bool>.Run(
                        () => UserSessionSingleton.Instance.GetHeartBeat()
                        );

            return ok;
        }

        /// <summary>
        /// Met à jour les messages
        /// </summary>
        private async void UpdateMessages(System.Windows.Controls.ListView lv)
        {
            using (LobbyService.LobbyServiceClient client = new LobbyService.LobbyServiceClient())
            {
                var lstMessages = await Task.Run(() => client.GetNewMessageFrom(UserSessionSingleton.Instance.UserToken.Value, _receiverName));

                bool newMsg = false;

                if (lstMessages.Count > 0)
                    newMsg = true;

                foreach (LobbyService.UserMessage um in lstMessages)
                    lv.Items.Add(um);

                if(lv.Items.Count > 0)
                    lv.ScrollIntoView(lv.Items[lv.Items.Count - 1]);

                if (newMsg)
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.MediaFailed += (o, args) =>
                    {
                        MessageBox.Show("Media Failed!!");
                    };
                    _mediaPlayer.Open(new Uri("Sounds/MSN Sound.mp3", UriKind.RelativeOrAbsolute));
                    _mediaPlayer.Play();

                    var helper = new FlashWindowHelper(Application.Current);
                    // Flashes the window and taskbar 5 times and stays solid 
                    // colored until user focuses the main window
                    helper.FlashApplicationWindow(); 
                }
            }
        }

        /// <summary>
        /// Action lorsqu'on envoie un message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (txtMessage.Text.Trim().Length < 1)
                return;

            btnSend.IsEnabled = false;

            bool ok = false;

            using (LobbyService.LobbyServiceClient client = new LobbyService.LobbyServiceClient())
            {
                ok = client.SendPrivateMessage(UserSessionSingleton.Instance.UserToken.Value, _receiverName, txtMessage.Text.Trim());
            }

            if (!ok)
            {
                MessageBox.Show("Erreur d'envoie du message, veuillez réessayer!");
            }
            else
            {
                LobbyService.UserMessage um = new LobbyService.UserMessage();
                um.Content = txtMessage.Text;
                um.Date = DateTime.UtcNow;
                um.Username = UserSessionSingleton.Instance.Name;
                lvMessages.Items.Add(um);

                txtMessage.Text = "";
            }

            btnSend.IsEnabled = true;
        }

        /// <summary>
        /// Lorsqu'on clique sur une touche (utilisé pour Enter)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnSend_Click(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Lorsqu'on ferme la fenêtre on notifie le lobby que c'est le cas.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            _attachedLobby.RemovePrivateMessageWindow(this);
        }
    }
}
