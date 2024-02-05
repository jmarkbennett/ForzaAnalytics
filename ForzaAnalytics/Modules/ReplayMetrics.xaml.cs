using ForzaAnalytics.UdpReader.Model;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for CoreMetrics.xaml
    /// </summary>
    public partial class ReplayMetrics : UserControl
    {

        public ReplayMetrics()
        {
            InitializeComponent();
        }

        public void ReceiveEvents(Models.Core.ExtendedPositionalData payload)
        {
            mAcc.Update($"{payload.Acceleration.ToString()}%");
            mBrake.Update($"{payload.Brake.ToString()}%");
            mClutch.Update($"{payload.Clutch.ToString()}%");
            mHandBrake.Update($"{payload.Handbrake.ToString()}%");
            mLapNumber.Update(payload.LapNumber.ToString());
            mLapTime.Update(Models.Formatters.Formatting.FormattedTime(payload.LapTime));
            mRaceTime.Update(Models.Formatters.Formatting.FormattedTime(payload.RaceTime));
            mSpeed.Update(payload.Speed_Mph.ToString("N"), payload.GearNumber, "MPH");
        }
        public void ResetEvents()
        {
            mAcc.Update(string.Empty);
            mBrake.Update(string.Empty);
            mClutch.Update(string.Empty);
            mHandBrake.Update(string.Empty);
            mLapNumber.Update(string.Empty);
            mLapTime.Update(string.Empty);
            mRaceTime.Update(string.Empty);
            mSpeed.Update(string.Empty, string.Empty, string.Empty);
        }
    }
}