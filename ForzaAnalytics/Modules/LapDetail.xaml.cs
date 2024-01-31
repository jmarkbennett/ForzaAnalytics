using System.Collections.ObjectModel;
using System.Windows.Controls;
using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Service;
using ForzaAnalytics.UdpReader.Model;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for LapDetail.xaml
    /// </summary>
    public partial class LapDetail : UserControl
    {
        private LapDetailService svc;
        public LapDetail()
        {
            svc = new LapDetailService();
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {
            tbCurrentLapNumber.Content = payload.Race.LapNumber.ToString();
            tbCurrentLapTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.CurrentLapTime);
            tbBestLapTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.BestLapTime);
            tbFuel.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Fuel);
            svc.Update(payload);
           
            lvLapTimes.ItemsSource = svc.LapTimes;
        }
        public void ResetEvents()
        {
            svc.ResetService();
            tbCurrentLapNumber.Content = string.Empty;
            tbCurrentLapTime.Content = string.Empty;
        }
    }
}
