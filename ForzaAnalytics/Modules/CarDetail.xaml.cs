using ForzaAnalytics.Models.Formatters;
using ForzaAnalytics.UdpReader.Model;
using System.Windows.Controls;
using ForzaAnalytics.Services.Helpers;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for CarDetail.xaml
    /// </summary>
    public partial class CarDetail : UserControl
    {
        public CarDetail()
        {
            InitializeComponent();
        }
        

        public void ReceiveEvents(Telemetry payload)
        {
            lCarClass.Content = Formatting.GetCarClass(payload.Car.CarClass).ToString();
            lCpi.Content = payload.Car.CarPerformanceIndex;
            lCarCylinders.Content = payload.Car.NumberOfCylinders;

            var color = Helpers.ColourHelper.GetColourFromString(
                ColourHelper.GetCarClassColour(
                    Formatting.GetCarClass(payload.Car.CarClass)));
                
            bCarClass.BorderBrush = color;
            rCarClass.Fill = color;
            lCarDriveTrain.Content = Formatting.GetDriveTrain(payload.Car.DrivetrainType).ToString();
        }
        public void ResetEvents()
        {
            lCarClass.Content = string.Empty;
            lCpi.Content = string.Empty;
            lCarCylinders.Content = string.Empty;
        }
    }
}
