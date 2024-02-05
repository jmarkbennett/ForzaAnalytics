using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class Configure : Window
    {
        public Configure()
        {
            var start = false;
            var useMetric = true;
            InitializeComponent();
            tbIpAddress.Text = ConfigurationManager.AppSettings["ListeningIpAddress"].ToString();
            tbPort.Text = ConfigurationManager.AppSettings["ListeningPort"].ToString();
            cbUseMetric.IsChecked = useMetric;
            cbTrackOnOpen.IsChecked = start;
        }

        private void btnUpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["UseMetric"].Value = cbUseMetric.IsChecked.ToString();
            config.AppSettings.Settings["TrackOnOpen"].Value = cbTrackOnOpen.IsChecked.ToString();
            config.AppSettings.Settings["ListeningIpAddress"].Value = tbIpAddress.Text;
            config.AppSettings.Settings["ListeningPort"].Value = tbPort.Text;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
