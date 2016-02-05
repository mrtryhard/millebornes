using Lobby.UserService;
using System;
using System.Windows;

namespace Lobby.Views
{
    /// <summary>
    /// Logique d'interaction pour RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Lorsqu'on veut s'enregistrer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            string sErreurs = "";

            // Vérifications de bases...
            if (txtUtilisateur.Text.Trim().Length < 3)
                sErreurs += "-Le nom d'utilisateur doit être minimalement 3 caractères de long.\n";

            if (txtPasse.Password.Trim().Length < 3)
                sErreurs += "-Le mot de passe doit être minimalement 3 caractères de long.\n";

            if (txtEmail.Text.Trim().Length < 7)
                sErreurs += "-L'adresse email est invalide :^(\n";

            using (var svcClient = new UserServiceClient())
            {
                CreateUserInfo cui = new CreateUserInfo();
                cui.Email = txtEmail.Text;
                cui.Password = txtPasse.Password;
                cui.Username = txtUtilisateur.Text;

                bool ok = svcClient.CreateUser(cui);

                if (!ok)
                    sErreurs += "-Utilisateur existant ou email déjà en utilisation.\n" +
                                "-Serveur peut-être hors-ligne?";
            }

            if (sErreurs.Length > 0)
            {
                MessageBox.Show(sErreurs, "Erreur");
                IsEnabled = true;
                return;
            }
            else
            {
                MessageBox.Show("Inscription réussie! vous pouvez désormais vous connecter.");
                Close();
            }
        }
    }
}
