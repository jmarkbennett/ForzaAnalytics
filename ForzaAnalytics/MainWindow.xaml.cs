using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using ForzaAnalytics.UdpReader.Service;
using System.Configuration;
using System.Diagnostics.Metrics;
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
        private Models.Enumerators.MessageRate messageRate = Models.Enumerators.MessageRate.Full;
        private int msgCounter = 0;
        private int msgLimit = 0;

        private Telemetry previousValue;
        public MainWindow()
        {
            InitializeComponent();
            bool.TryParse(ConfigurationManager.AppSettings["TrackOnOpen"]?.ToString(), out bool start);
            bool.TryParse(ConfigurationManager.AppSettings["UseMetric"]?.ToString(), out bool useMetric);
            Enum.TryParse(ConfigurationManager.AppSettings["MessageRate"]?.ToString(), out Models.Enumerators.MessageRate messageRate);
            bool.TryParse(ConfigurationManager.AppSettings["ClearStats"]?.ToString(), out bool clearStats);
            bool.TryParse(ConfigurationManager.AppSettings["SaveStats"]?.ToString(), out bool saveStats);

            SessionSerializer.initializeDatabase();
            if (clearStats)
            {
                SessionSerializer.ResetDatabase();
            }

            mSessionManager.SaveStats = saveStats;

            SetMessageLimit(messageRate);
            if (start)
            {
                miIsListening.IsChecked = true;
                ToggleListen(true);
            }
        }

        private void SetMessageLimit(Models.Enumerators.MessageRate msgRate)
        {
            switch (msgRate)
            {
                case Models.Enumerators.MessageRate.Full:
                    msgLimit = msgCounter = 0;
                    break;
                case Models.Enumerators.MessageRate.Medium:
                    msgLimit = msgCounter = 2;
                    break;
                case Models.Enumerators.MessageRate.Low:
                    msgLimit = msgCounter = 3;
                    break;
                case Models.Enumerators.MessageRate.Lower:
                    msgLimit = msgCounter = 4;
                    break;
                case Models.Enumerators.MessageRate.Lowest:
                    msgLimit = msgCounter = 5;
                    break;
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
                        if (msgCounter == 0)
                        {
                            if (mSessionDetails.Visibility == Visibility.Visible)
                                mSessionDetails.ReceiveEvents(payload);
                            if (tLapTimes.IsChecked)
                            {
                                if (previousValue != null && previousValue.isReportingActive && !payload.isReportingActive)
                                    mSessionManager.ReceiveEvents(previousValue, true);
                                else
                                    mSessionManager.ReceiveEvents(payload, false);
                            }
                            if (tLapTimes.IsChecked)


                                if (payload.isReportingActive)
                                {
                                    if (mPedalPressures.Visibility == Visibility.Visible)
                                        mPedalPressures.ReceiveEvents(payload);
                                    if (mCoreMetrics.Visibility == Visibility.Visible)
                                        mCoreMetrics.ReceiveEvents(payload);

                                    if (tMapGenerator.IsChecked)
                                        mMapGenerator.ReceiveEvents(payload);
                                    if (tCarPositions.IsChecked)
                                        mPositionMap.ReceiveEvents(payload);
                                    if (tAllMetrics.IsChecked)
                                        mAllMetrics.ReceiveEvents(payload);
                                }

                            if (tTrackTelemetry.IsChecked)
                                mTelemetryLog.ReceiveEvents(payload);
                            msgCounter = msgLimit;

                            previousValue = payload;
                        }
                        else
                            msgCounter -= 1;
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
                mPedalPressures.Visibility == Visibility.Collapsed
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
                eTrackTelemetry.Fill = tTrackTelemetry.IsChecked ? new SolidColorBrush(Colors.YellowGreen) : new SolidColorBrush(Colors.OrangeRed);
            }
            else
            {
                eLapTimes.Fill = new SolidColorBrush(Colors.DarkGray);
                eAllMetrics.Fill = new SolidColorBrush(Colors.DarkGray);
                eMapGenerator.Fill = new SolidColorBrush(Colors.DarkGray);
                ePositionMap.Fill = new SolidColorBrush(Colors.DarkGray);
                eTrackTelemetry.Fill = new SolidColorBrush(Colors.DarkGray);
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
            mCoreMetrics.ResetEvents();
            // mSessionManager.ResetEvents();
            mMapGenerator.ResetEvents();
            mPositionMap.ResetEvents();
            mAllMetrics.ResetEvents(); ;
        }
    }
}