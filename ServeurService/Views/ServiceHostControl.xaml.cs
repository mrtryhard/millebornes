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

using System.ServiceModel;

namespace ServeurService.Views
{
    /// <summary>
    /// Interaction logic for ServiceHostControl.xaml
    /// </summary>
    public partial class ServiceHostControl : UserControl
    {
        public Type ServiceType { get; set; }

        private ServiceHost _serviceHoster = null;

        public ServiceHostControl()
        {
            InitializeComponent();
        }

        public void InitService()
        {
            _serviceHoster = new ServiceHost(ServiceType);

            _serviceHoster.Opened += _serviceHoster_Opened;
            _serviceHoster.Closed += _serviceHoster_Closed;
            _serviceHoster.Faulted += _serviceHoster_Faulted;

            try
            {
                _serviceHoster.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors du lancement du service: " + ex.Message
                );
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
            if (_serviceHoster == null)
            {
                return;
            }

            _serviceHoster.Close();
        }
    }
}
