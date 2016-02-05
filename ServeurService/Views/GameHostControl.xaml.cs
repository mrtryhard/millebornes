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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ServiceModel;

using LibrairieService.Services;

namespace ServeurService.Views
{
    /// <summary>
    /// Interaction logic for GameHostControl.xaml
    /// </summary>
    public partial class GameHostControl : UserControl
    {
        private ServiceHost _serviceHoster;

        public GameHostControl()
        {
            InitializeComponent();
        }

        public void InitService()
        {
            GameService.ErrorOccurred += GameService_ErrorOccurred;
            GameService.EventOccurred += GameService_EventOccurred;

            _serviceHoster = new ServiceHost(typeof(LibrairieService.Services.GameService));

            _serviceHoster.Opened += _serviceHoster_Opened;
            _serviceHoster.Closed += _serviceHoster_Closed;
            _serviceHoster.Faulted += _serviceHoster_Faulted;

            try
            {
                _serviceHoster.Open();
            }
            catch (Exception ex)
            {
                throw;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in _serviceHoster.Description.Endpoints)
            {
                sb.Append(string.Format(
                    "Contrat {0}:\n\tBinding: {1}\r\tAdresse: {2}\n",
                    item.Contract.ContractType.FullName,
                    item.Binding.Name,
                    item.Address
                ));
            }
            lblEndpoints.Content = sb.ToString();

            lblServiceName.Content = string.Format(
                "Service {0}",
                _serviceHoster.Description.ConfigurationName
            );
        }

        void GameService_EventOccurred(LogLevel arg1, string arg2)
        {
            LogMessageToScreen(arg1, arg2);
        }

        void GameService_ErrorOccurred(LogLevel arg1, string arg2)
        {
            LogMessageToScreen(arg1, arg2);
        }

        private void LogMessageToScreen(LogLevel level, string message)
        {
            txtServiceLog.Text += string.Format("[{0}] {1}\n", level, message);
        }

        private void DefineStatus(string status)
        {
            lblStatus.Content = string.Format(
                "État du service: {0}",
                status
            );
        }

        void _serviceHoster_Opened(object sender, EventArgs e)
        {
            DefineStatus("Ouvert");
        }

        void _serviceHoster_Faulted(object sender, EventArgs e)
        {
            DefineStatus("Erreur");
        }

        void _serviceHoster_Closed(object sender, EventArgs e)
        {
            DefineStatus("Fermé");
        }

        public void ShutdownService()
        {
            _serviceHoster.Close();
        }
    }
}
