using ForzaAnalytics.UdpReader.Service;
using System.Configuration;
using System.Net;
using System.Windows;

namespace ForzaAnalytics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TelemetryService svc;
        private System.Threading.Thread thread;
        private bool threadRunning = false;
        private Task task;
        public MainWindow()
        {
            InitializeComponent();
            var start = false;
            var useMetric = true;
            bool.TryParse(ConfigurationManager.AppSettings["TrackOnOpen"].ToString(), out start);
            bool.TryParse(ConfigurationManager.AppSettings["UseMetric"].ToString(), out useMetric);
            tbIpAddress.Text = ConfigurationManager.AppSettings["ListeningIpAddress"].ToString();
            tbPort.Text = ConfigurationManager.AppSettings["ListeningPort"].ToString();
            cbUseMetric.IsChecked = useMetric;
            cbTrackOnOpen.IsChecked = start;

            if (start)
                tglTracking.IsChecked = true;
        }

        private void ReceiveEvents()
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (threadRunning)
                {
                    var payload = svc.ReceiveEvents();
                    // Update the UI from the UI thread using Dispatcher
                    Dispatcher.Invoke(() =>
                    {
                        sessionDetailsModule.ReceiveEvents(payload);

                        if (payload.isReportingActive)
                        {
                            ldlapDetail.ReceiveEvents(payload);
                            ppAbc.ReceiveEvents(payload);
                            carDetailsModule.ReceiveEvents(payload);
                            mapGenerator.ReceiveEvents(payload);
                            positionMap.ReceiveEvents(payload);
                            coreMetricsModule.ReceiveEvents(payload);
                            allMetrics.ReceiveEvents(payload);
                        }
                        else
                        {
                            //ldlapDetail.ResetEvents();
                            //ppAbc.ResetEvents();
                            //carDetailsModule.ResetEvents();
                            //mapGenerator.ResetEvents();
                            //positionMap.ResetEvents();
                            //coreMetricsModule.ResetEvents();
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void tglTracking_Checked(object sender, RoutedEventArgs e)
        {
            tglTracking.Content = "Stop";
            if (!threadRunning)
            {
 
                if (svc == null)
                    svc = new TelemetryService(
                        ConfigurationManager.AppSettings["ListeningIpAddress"].ToString(), 
                        int.Parse(ConfigurationManager.AppSettings["ListeningPort"].ToString())
                        );
                threadRunning = true;
                thread = new Thread(ReceiveEvents);
                thread.Start();
            }
        }
        private void tglTracking_Unchecked(object sender, RoutedEventArgs e)
        {
            tglTracking.Content = "Start";
            if (thread == null || thread.ThreadState == ThreadState.Running)
            {
                threadRunning = false;
            }
        }
        private void btnResetAll_Click(object sender, RoutedEventArgs e)
        {
           if(MessageBox.Show("Are you sure you want to Reset ALL controls? this removes any data currently loaded and should only be done on the start of a new session", "Confirm Reset", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ldlapDetail.ResetEvents();
                ppAbc.ResetEvents();
                carDetailsModule.ResetEvents();
                mapGenerator.ResetEvents();
                positionMap.ResetEvents();
                coreMetricsModule.ResetEvents();
                allMetrics.ResetEvents();
            }
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