using ForzaAnalytics.UdpReader.Service;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();
            bool.TryParse(ConfigurationManager.AppSettings["TrackOnOpen"]?.ToString(), out bool start);
            bool.TryParse(ConfigurationManager.AppSettings["UseMetric"]?.ToString(), out bool useMetric);

            if (start)
            {
                miIsListening.IsChecked = true;
                ToggleListen(true);
            }
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
                        if (payload.isReportingActive)
                        {
                            if (mSessionDetails.Visibility == Visibility.Visible)
                                mSessionDetails.ReceiveEvents(payload);
                            if (mPedalPressures.Visibility == Visibility.Visible)
                                mPedalPressures.ReceiveEvents(payload);
                            if (mCarDetails.Visibility == Visibility.Visible)
                                mCarDetails.ReceiveEvents(payload);
                            if (mCarDetails.Visibility == Visibility.Visible)
                                mCoreMetrics.ReceiveEvents(payload);

                            if (tLapTimes.IsChecked)
                                mlapDetail.ReceiveEvents(payload);
                            if (tMapGenerator.IsChecked)
                                mMapGenerator.ReceiveEvents(payload);
                            if (tCarPositions.IsChecked)
                                mPositionMap.ReceiveEvents(payload);
                            if (tAllMetrics.IsChecked)
                                mAllMetrics.ReceiveEvents(payload);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ToggleModuleVisibility(object sender, RoutedEventArgs e)
        {
            var module = ((MenuItem)sender).Header;
            switch (module)
            {
                case "Show Session Summary":
                    if (mSessionDetails.Visibility == Visibility.Visible)
                        mSessionDetails.Visibility = Visibility.Collapsed;
                    else
                        mSessionDetails.Visibility = Visibility.Visible;
                    break;
                case "Show Core Metrics":
                    if (mCoreMetrics.Visibility == Visibility.Visible)
                        mCoreMetrics.Visibility = Visibility.Collapsed;
                    else
                        mCoreMetrics.Visibility = Visibility.Visible;
                    break;
                case "Show Car Summary":
                    if (mCarDetails.Visibility == Visibility.Visible)
                        mCarDetails.Visibility = Visibility.Collapsed;
                    else
                        mCarDetails.Visibility = Visibility.Visible;
                    break;
                case "Show Pedal Pressures":
                    if (mPedalPressures.Visibility == Visibility.Visible)
                        mPedalPressures.Visibility = Visibility.Collapsed;
                    else
                        mPedalPressures.Visibility = Visibility.Visible;
                    break;
            }
            // Handle Visibility of Rows based on whats hidden
            if (
                mSessionDetails.Visibility == Visibility.Collapsed &&
                mPedalPressures.Visibility == Visibility.Collapsed &&
                mCarDetails.Visibility == Visibility.Collapsed
                )
                TopMenu.Visibility = Visibility.Collapsed;
            else
                TopMenu.Visibility = Visibility.Visible;
            RefreshIcons();
        }

        private void IsListening_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = ((MenuItem)sender).IsChecked;
            ToggleListen(isChecked);
        }

        private void ToggleListen(bool isChecked)
        {
            if (isChecked)
            {
                if (!threadRunning)
                {
                    if (svc == null)
                        svc = new TelemetryService(
                            ConfigurationManager.AppSettings["ListeningIpAddress"]?.ToString() ?? "127.0.0.1",
                            int.Parse(ConfigurationManager.AppSettings["ListeningPort"]?.ToString() ?? "6969")
                            );
                    threadRunning = true;
                    thread = new Thread(ReceiveEvents);
                    thread.Start();
                }
            }
            else
            {
                if (thread == null || thread.ThreadState == ThreadState.Running)
                {
                    threadRunning = false;
                }
            }
            RefreshIcons();
        }

        private void RefreshIcons()
        {
            if (threadRunning)
            {
                eLapTimes.Fill = tLapTimes.IsChecked ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.OrangeRed);
                eAllMetrics.Fill = tAllMetrics.IsChecked ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.OrangeRed);
                eMapGenerator.Fill = tMapGenerator.IsChecked ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.OrangeRed);
                ePositionMap.Fill = tCarPositions.IsChecked ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.OrangeRed);
            }
            else
            {
                eLapTimes.Fill = new SolidColorBrush(Colors.DarkGray);
                eAllMetrics.Fill = new SolidColorBrush(Colors.DarkGray);
                eMapGenerator.Fill = new SolidColorBrush(Colors.DarkGray);
                ePositionMap.Fill = new SolidColorBrush(Colors.DarkGray);
            }
        }

        private void tToggleTrack_Click(object sender, RoutedEventArgs e)
        {
            RefreshIcons();
        }

        private void miConfigure_Click(object sender, RoutedEventArgs e)
        {
            Modules.Configure configure = new Modules.Configure();
            configure.Show();
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            mSessionDetails.ResetEvents();
            mPedalPressures.ResetEvents();
            mCarDetails.ResetEvents();
            mCoreMetrics.ResetEvents();
            mlapDetail.ResetEvents();
            mMapGenerator.ResetEvents();
            mPositionMap.ResetEvents();
            mAllMetrics.ResetEvents(); ;
        }
    }
}