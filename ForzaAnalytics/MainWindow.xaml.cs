using ForzaAnalytics.UdpReader.Service;
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
                    svc = new TelemetryService(tbIpAddress.Text, int.Parse(tbPort.Text));
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
    }
}