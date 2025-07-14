using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Formatters;
using ForzaAnalytics.Services.Helpers;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System.Windows.Controls;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for CarDetail.xaml
    /// </summary>
    public partial class CarDetail : UserControl
    {
        private List<Models.Core.Car> carDetails;
        private List<Models.Core.Track>  trackDetails;
        private string carId = string.Empty;
        private string trackId = string.Empty;
        public CarDetail()
        {
            InitializeComponent();
            carDetails = CarDetailsSeralizer.LoadCarDetails();
            trackDetails = TrackDetailsSeralizer.LoadTrackDetails();
        }
        

        public void ReceiveEvents(Telemetry payload)
        {
            if(payload.Car.CarIdentifier.ToString() != carId && carDetails.Any())
            {
                carId = payload.Car.CarIdentifier.ToString();
                var carName = carDetails.Where(i=>i.CarId == carId).Select(i=>i.YearMakeModel).FirstOrDefault();
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
            lCarClass.Content = string.Empty;
            lCpi.Content = string.Empty;
            lCarName.Content = string.Empty;
            //lCarCylinders.Content = string.Empty;
            //lCarDriveTrain.Content = string.Empty;
        }
    }
}
