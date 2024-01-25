using ForzaAnalytics.UdpReader.Model;
using System.Windows.Controls;
using System.Windows.Media;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for SessionDetail.xaml
    /// </summary>
    public partial class SessionDetail : UserControl
    {
        public bool IsReportingActive { get; set; }

       
        public SessionDetail()
        {
            IsReportingActive = false;
            InitializeComponent();
        }

        public void ReceiveEvents(Telemetry payload)
        {
            IsReportingActive = payload.isReportingActive;
            if (payload.isReportingActive)
                eGameStatus.Fill = Brushes.Green;
            else
                eGameStatus.Fill = Brushes.Red;
            lGameStatus.Content = payload.isReportingActive ? "Running" : "Paused";
            lSessionTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.CurrentRaceTime);
            if (payload.Race.BestLapTime > 0)
                lBestTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.BestLapTime);
            else
            {
                lBestTime.Content = "Pending...";
            }
            lRacePosition.Content = payload.Race.RacePosition;
        }
        public void ResetEvents()
        {
            lGameStatus.Content = string.Empty;
            lSessionTime.Content = string.Empty;
            lBestTime.Content = string.Empty;
            lRacePosition.Content = string.Empty;
        }
    }
}
