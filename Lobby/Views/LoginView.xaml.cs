using Lobby.Model;
using Lobby.UserService;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lobby.Views
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private LobbyView _lobby;

        public LoginView()
        {
            InitializeComponent();
            LoadConnections();
            _lobby = new LobbyView();
        }

        /// <summary>
        /// Petit hacks pour préloader les connexions et certains symboles.
        /// </summary>
        private async void LoadConnections()
        {
            await Task.Delay(1000);

            try
            {
                LobbyService.LobbyServiceClient ddd = new LobbyService.LobbyServiceClient();
                ddd.Open();
                UserServiceClient ccc = new UserServiceClient();
                ccc.Open();

                using (var gsc = new GameProxy.GameServiceClient())
                {
                    gsc.Open();
                }
            }
            catch
            {
                MessageBox.Show("Il semble que la connexion est inexistante...");
            }
        }

        /// <summary>
        /// Action lorsqu'on clique sur "Connexion"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            string sErreurs = "";

            // Vérifications de bases...
            if (txtUtilisateur.Text.Trim().Length < 3)
                sErreurs += "-Le nom d'utilisateur doit être minimalement 3 caractères de long.\n";

            if (txtPasse.Password.Trim().Length < 3)
                sErreurs += "-Le mot de passe doit être minimalement 3 caractères de long.\n";

            if (sErreurs.Length <= 0)
            {
                using (var svcClient = new UserServiceClient())
                {
                    Guid? guid = null;

                    try
                    {
                        guid = svcClient.Login(txtUtilisateur.Text.Trim(), txtPasse.Password);
                    }
                    catch
                    {
                        sErreurs += "-Le serveur est hors-ligne.\n"; 
                    }

                    if (guid == null)
                        sErreurs += "-Les identifiants sont erronés\n";

                    UserSessionSingleton.Instance.UserToken = guid;
                    UserSessionSingleton.Instance.Name = txtUtilisateur.Text;
                }
            }

            if (sErreurs.Length > 0)
            {
                MessageBox.Show(sErreurs, "Erreur");
                IsEnabled = true;
            }
            else
            {
                OpenLobby();
            }
        }

        /// <summary>
        /// Ouvre le lobby.
        /// </summary>
        private void OpenLobby()
        {
            _lobby.Show();
            _lobby.StartUpdating();

            Close();
        }

        /// <summary>
        /// Lorsqu'on clique sur inscription.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblInscription_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterView rv = new RegisterView();
            rv.ShowDialog();
        }
    }
}
