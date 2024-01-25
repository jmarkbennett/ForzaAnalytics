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
            pbAcc.Value = payload.Acceleration;
            pbBrake.Value = payload.Brake;
            pbClutch.Value = payload.Clutch;
            pbHandbrake.Value = payload.Handbrake;
            pbRpms.Value = payload.EngineRpm;
            pbFuel.Value = payload.Fuel;

            pbRpms.Maximum = payload.EngineMaxRpm;

            if (pbRpms.Value > (pbRpms.Maximum * .85))
            {
                tbRpms.Foreground = Brushes.White;
                pbRpms.Foreground = Brushes.Red;
            }
            else if(pbRpms.Value < (pbRpms.Maximum * .60))
            {
                tbRpms.Foreground = Brushes.White;
            }
            else
            {
                tbRpms.Foreground = Brushes.White;
                pbRpms.Foreground = Brushes.LightGray;
            }
            tbAcc.Content = payload.Acceleration.ToString();
            tbBrake.Content = payload.Brake.ToString();
            tbClutch.Content = payload.Clutch.ToString();
            tbRpms.Content = payload.EngineRpm.ToString("N");
            tbHandbrake.Content = payload.Handbrake.ToString();
            tbFuel.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Fuel);
        }
        public void ResetEvents()
        {
            pbAcc.Value = 0;
            pbBrake.Value = 0;
            pbClutch.Value = 0;
            tbAcc.Content = string.Empty;
            tbBrake.Content = string.Empty;
            tbClutch.Content = string.Empty;
        }
    }
}
