using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.Services.Service;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for LapDetail.xaml
    /// </summary>
    public partial class SessionManager : UserControl
    {
        private SessionService svc;
        private LapDetailService ldSvc;

        public SessionManager()
        {
            svc = new SessionService();
            ldSvc = new LapDetailService();

            InitializeComponent();
            lvLapTimes.ItemsSource = ldSvc.ReversedLapTimes;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLapTimes.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SessionSummary");
            view.GroupDescriptions.Add(groupDescription);
        }
        public void ReceiveEvents(Telemetry payload)
        {
            svc.Update(payload);
            var prvLaps = ldSvc.LapTimes.Count();
            if(prvLaps != ldSvc.LapTimes.Count()){
                lvLapTimes.ItemsSource = ldSvc.ReversedLapTimes;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLapTimes.ItemsSource);
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("SessionSummary");
                view.GroupDescriptions.Add(groupDescription);
            }
        }

        public static System.Windows.Media.Brush GetTyreWearColour(float wear)
        {
            if (wear < 0.2)
                return new SolidColorBrush(Colors.DarkRed);
            if (wear < 0.3)
                return new SolidColorBrush(Colors.Red);
            if (wear < 0.4)
                return new SolidColorBrush(Colors.OrangeRed);
            if (wear < 0.5)
                return new SolidColorBrush(Colors.Yellow);
            if (wear < 0.6)
                return new SolidColorBrush(Colors.LightYellow);
            else
                return new SolidColorBrush(Colors.White);
        }

        private void btnAbandonSession_Click(object sender, RoutedEventArgs e)
        {
            var endDate = DateTime.Now;
            if (svc.CurrentLapTimes.Any())
                endDate = svc.CurrentLapTimes.Last().TimeOfLapTime;
            SessionSerializer.CloseSession(svc.SessionId, "ManualAbandon", endDate);
            svc.Reset();
        }

        private void btnCompleteSession_Click(object sender, RoutedEventArgs e)
        {
            var endDate = DateTime.Now;
            if (svc.CurrentLapTimes.Any())
                endDate = svc.CurrentLapTimes.Last().TimeOfLapTime;
            SessionSerializer.CloseSession(svc.SessionId, "ManualComplete", endDate);
            svc.Reset();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Export Session Data",
                FileName = "data.csv"
            };
            var result = dialog.ShowDialog();
            if(dialog.FileName != "")
            {
                SessionSerializer.ExportSessionData(dialog.FileName);
            }
 
        }
    }
}
