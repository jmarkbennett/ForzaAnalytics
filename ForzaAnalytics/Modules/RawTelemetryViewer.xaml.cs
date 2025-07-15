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
    public partial class RawTelemetryViewer : UserControl
    {


        public RawTelemetryViewer()
        {
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {

            var row = $"LapNumber:{payload.Race.LapNumber}|RaceTime:{payload.Race.CurrentRaceTime}|LapTime:{payload.Race.CurrentLapTime}";

            tbOutput.Text = row  + "\n" + tbOutput.Text;

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbOutput.Text = "";
        }
    }
}
