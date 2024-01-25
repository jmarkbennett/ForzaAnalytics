using ForzaAnalytics.UdpReader.Model;
using System.Windows.Controls;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for CoreMetrics.xaml
    /// </summary>
    public partial class CoreMetrics : UserControl
    {
        public CoreMetrics()
        {
            InitializeComponent();
        }

        public void ReceiveEvents(Telemetry payload)
        {
            lBoost.Content = payload.Boost.ToString("N");
            lFuel.Content = ForzaAnalytics.Models.Formatters.Formatting.FormattedPercentage(payload.Fuel);
            lGear.Content = payload.GearNumber;
            lPitch.Content = payload.Pitch.ToString("N");
            lYaw.Content = payload.Yaw.ToString("N");
            lRoll.Content = payload.Roll.ToString("N");
            lRpm.Content = payload.EngineRpm.ToString("N");
            lSpeed.Content = payload.Speed_Mph;
            lPower.Content = payload.EnginePowerHp.ToString("N");
            lTorque.Content= payload.TorqueOneFootPound.ToString("N");
            lDistanceTravelled.Content = payload.DistanceTravelled.ToString("N");
        }
        public void ResetEvents()
        {

        }
    }
}
