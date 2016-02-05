using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ServeurService.Views
{
    /// <summary>
    /// Interaction logic for HostView.xaml
    /// </summary>
    public partial class HostView : Window
    {
        ServiceHostControl[] _allGenericServiceHosts;
        public HostView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            shcUserService.ServiceType = typeof(LibrairieService.Services.UserService);
            shcLobbyServiceWsHttp.ServiceType = typeof(LibrairieService.Services.LobbyService);
            shcLobbyServiceTcp.ServiceType = typeof(LibrairieService.Services.LobbyGameService);

            _allGenericServiceHosts = new[] 
            {
                shcUserService,
                shcLobbyServiceWsHttp,
                shcLobbyServiceTcp
            };

            foreach (var serviceHost in _allGenericServiceHosts)
            {
                serviceHost.InitService();
            }
            ghcGameService.InitService();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var serviceHost in _allGenericServiceHosts)
            {
                serviceHost.ShutdownService();
            }
            ghcGameService.ShutdownService();
        }
    }
}
