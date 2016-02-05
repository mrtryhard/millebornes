using Lobby.Model;
using System.Windows;

namespace Lobby.Views
{
    /// <summary>
    /// Logique d'interaction pour ClientInfosView.xaml
    /// </summary>
    public partial class ClientInfosView : Window
    {
        public ClientInfosView()
        {
            InitializeComponent();
            lblGuid.Text = UserSessionSingleton.Instance.UserToken.Value.ToString();
            lblLastLogin.Text = "";
            lblPseudo.Text = UserSessionSingleton.Instance.Name;
        }
    }
}
