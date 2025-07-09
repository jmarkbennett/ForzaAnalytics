using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System.Windows.Controls;
using System.Windows.Media;
using ForzaAnalytics.Models.Formatters;
using ForzaAnalytics.Services.Helpers;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for SessionDetail.xaml
    /// </summary>
    public partial class SessionDetail : UserControl
    {
        public bool IsReportingActive { get; set; }
        private List<Models.Core.CarDetail> carDetails;
        private List<Models.Core.TrackDetail> trackDetails;
        private string carId = string.Empty;
        private string trackId = string.Empty;

        public SessionDetail()
        {
            IsReportingActive = false;
            carDetails = CarDetailsSeralizer.LoadCarDetails($"{Environment.CurrentDirectory}\\Resources\\Files\\ManteoMaxs FM Spreadsheet.csv");
            trackDetails = TrackDetailsSeralizer.LoadTrackDetails($"{Environment.CurrentDirectory}\\Resources\\Files\\Track Ordinals.csv");
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
            lDistanceTravelled.Content = $"{payload.DistanceTravelled_Km}KM";
            if (payload.Race.BestLapTime > 0)
                lBestTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.BestLapTime);
            else
            {
                lBestTime.Content = "Pending...";
            }
            lRacePosition.Content = payload.Race.RacePosition;

            if (payload.Car.CarIdentifier.ToString() != carId && carDetails.Any())
            {
                carId = payload.Car.CarIdentifier.ToString();
                var carName = carDetails.Where(i => i.CarId == carId).Select(i => i.YearMakeModel).FirstOrDefault();
                lCarName.Content = carName;
            }

            if (payload.Race.TrackIdentifier.ToString() != trackId && trackDetails.Any())
            {
                trackId = payload.Race.TrackIdentifier.ToString();
                var trackName = trackDetails.Where(i => i.TrackId == trackId).Select(i => i.FullTrackName).FirstOrDefault();
                lTrackName.Content = trackName;
            }

            lCarClass.Content = Formatting.GetCarClass(payload.Car.CarClass).ToString();
            lCpi.Content = payload.Car.CarPerformanceIndex;

            var color = Helpers.ColourHelper.GetColourFromString(
                ColourHelper.GetCarClassColour(
                    Formatting.GetCarClass(payload.Car.CarClass)));

            bCarClass.BorderBrush = color;
            rCarClass.Fill = color;

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
