using ForzaAnalytics.UdpReader.Model;
using System.Diagnostics;
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
            mBoost.Update(payload.Boost.ToString("N"));
            mFuel.Update(Models.Formatters.Formatting.FormattedPercentage(payload.Fuel));
            mPitch.Update(payload.Pitch.ToString("N"));
            mYaw.Update(payload.Yaw.ToString("N"));
            mRoll.Update(payload.Roll.ToString("N"));
            mRpm.Update(payload.EngineRpm.ToString("N"));
            mPower.Update(payload.EnginePowerHp.ToString("N"));
            mTorque.Update(payload.TorqueOneFootPound.ToString("N"));
            mDistanceTravelled.Update(payload.DistanceTravelled.ToString("N"));
            mSteeringAngle.Update(payload.SteeringAngle.ToString());
            mSpeed.Update(payload.Speed_Mph.ToString("N"), payload.GearNumber);
        }
        public void ResetEvents()
        {

        }
    }
}
