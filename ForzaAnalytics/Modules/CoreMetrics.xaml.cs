using ForzaAnalytics.UdpReader.Model;
using System.Configuration;
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
            bool useMetric = true;
            bool.TryParse(ConfigurationManager.AppSettings["UseMetric"], out useMetric);

            mBoost.Update(payload.Boost.ToString("N"));
            mFuel.Update(Models.Formatters.Formatting.FormattedPercentage(payload.Fuel));
            mPitch.Update(payload.Pitch.ToString("N"));
            mYaw.Update(payload.Yaw.ToString("N"));
            mRoll.Update(payload.Roll.ToString("N"));
            mRpm.Update(payload.EngineRpm.ToString("N"));
            mPower.Update(payload.EnginePowerHp.ToString("N"));
            mTorque.Update(payload.TorqueOneFootPound.ToString("N"));
            mDistanceTravelled.Update(payload.DistanceTravelled.ToString("N"));
            mSteeringAngle.Update(payload.SteeringAngle);
            mSpeed.Update(
                useMetric ? payload.Speed_Kph.ToString("N") : payload.Speed_Mph.ToString("N"), 
                payload.GearNumber,
                 useMetric ? "KPH" : "MPH"
                );
        }
        public void ResetEvents()
        {
            mBoost.Update(string.Empty);
            mFuel.Update(string.Empty);
            mPitch.Update(string.Empty);
            mYaw.Update(string.Empty);
            mRoll.Update(string.Empty);
            mRpm.Update(string.Empty);
            mPower.Update(string.Empty);
            mTorque.Update(string.Empty);
            mDistanceTravelled.Update(string.Empty);
            mSteeringAngle.Update(0);
            mSpeed.Update(string.Empty,string.Empty, string.Empty);
        }
    }
}
