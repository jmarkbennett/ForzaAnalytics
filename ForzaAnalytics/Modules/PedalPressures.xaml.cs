using ForzaAnalytics.UdpReader.Model;
using System.Windows.Controls;
using System.Windows.Media;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for PedalPressures.xaml
    /// </summary>
    public partial class PedalPressures : UserControl
    {
        public PedalPressures()
        {
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {
            gmAcceleration.Update(payload.Acceleration, payload.Acceleration.ToString());
            gmBrake.Update(payload.Brake, payload.Brake.ToString());
            gmClutch.Update(payload.Clutch, payload.Clutch.ToString());
            gmHandbrake.Update(payload.Handbrake, payload.Handbrake.ToString());
            gmRPM.Update(payload.EngineRpm, payload.EngineRpm.ToString("N"));
            gmFuel.Update(payload.Fuel, Models.Formatters.Formatting.FormattedPercentage(payload.Fuel));
            gmRPM.Maximum = payload.EngineMaxRpm;
        }
        public void ResetEvents()
        {

        }
    }
}
