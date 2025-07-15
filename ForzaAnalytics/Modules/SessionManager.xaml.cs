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

        public SessionManager()
        {
            svc = new SessionService();

            InitializeComponent();
            RebindListView();
        }

        private void RebindListView()
        {
            lvLapTimes.ItemsSource = svc.LapDetails.ReversedLapTimes;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLapTimes.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SessionSummary");
            view.GroupDescriptions.Add(groupDescription);
        }
        public void ReceiveEvents(Telemetry payload, bool sessionPotentiallyEnded)
        {
            if (!sessionPotentiallyEnded && payload.isReportingActive)
            {
                var prvLaps = svc.LapDetails.LapTimes.Count();
                svc.Update(payload);
                if (prvLaps != svc.LapDetails.LapTimes.Count())
                {
                    RebindListView();
                }
            }
            else if (sessionPotentiallyEnded)
            {
                var msg = MessageBox.Show("Do you want to Finish this session? (Only select YES if it was a Race and the final lap is complete)", "Session Status Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (msg == MessageBoxResult.Yes) // Yes for a race session...
                {
                    CompleteRaceSession();
                }
            }
        }

        public static Brush GetTyreWearColour(float wear)
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
            SessionSerializer.CloseSession(svc.CurrentSession.SessionId, "ManualAbandon", endDate);
            svc.Reset();
            svc.LapDetails.Reset();
            svc.LapDetails.SyncData();
            MessageBox.Show("Session Abandoned","Information",MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCompleteSession_Click(object sender, RoutedEventArgs e)
        {
            
            var endDate = DateTime.Now;
            if (svc.CurrentLapTimes.Any())
                endDate = svc.CurrentLapTimes.Last().TimeOfLapTime;
            SessionSerializer.CloseSession(svc.CurrentSession.SessionId, "ManualComplete", endDate);
            svc.Reset();
            svc.LapDetails.Reset();
            svc.LapDetails.SyncData();
            MessageBox.Show("Session Completed", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCompleteRaceSession_Click(object sender, RoutedEventArgs e)
        {
            CompleteRaceSession();
            MessageBox.Show("Final Lap Logged", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CompleteRaceSession()
        {
            var endDate = DateTime.Now;
            svc.CreateFinalRaceLap();
            if (svc.CurrentLapTimes.Any())
                endDate = svc.CurrentLapTimes.Last().TimeOfLapTime;
            SessionSerializer.CloseSession(svc.CurrentSession.SessionId, "ManualComplete", endDate);
            svc.Reset();
            svc.LapDetails.Reset();
            svc.LapDetails.SyncData();
            RebindListView();

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
