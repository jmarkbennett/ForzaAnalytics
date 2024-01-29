using ForzaAnalytics.Controls;
using ForzaAnalytics.Models.Formatters;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for AllMetrics.xaml
    /// </summary>
    public partial class AllMetrics : UserControl
    {
        public AllMetrics()
        {
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {
            mAcc.Update($"{payload.Acceleration.ToString()}%");
            mBrake.Update($"{payload.Brake.ToString()}%");
            mClutch.Update($"{payload.Clutch.ToString()}%");
            mHandbrake.Update($"{payload.Handbrake.ToString()}%");
            mNormSuspTravelFl.Update(payload.Suspension.NormalizedSuspensionTravelFrontLeft.ToString("F2"));
            mNormSuspTravelFr.Update(payload.Suspension.NormalizedSuspensionTravelFrontRight.ToString("F2"));
            mNormSuspTravelRl.Update(payload.Suspension.NormalizedSuspensionTravelRearLeft.ToString("F2"));
            mNormSuspTravelRr.Update(payload.Suspension.NormalizedSuspensionTravelRearRight.ToString("F2"));

            mWheelPuddleFl.Update(payload.Wheel.WheelInPuddleDepthFrontLeft.ToString());
            mWheelPuddleFr.Update(payload.Wheel.WheelInPuddleDepthFrontRight.ToString());
            mWheelPuddleRl.Update(payload.Wheel.WheelInPuddleDepthRearLeft.ToString());
            mWheelPuddleRr.Update(payload.Wheel.WheelInPuddleDepthRearRight.ToString());

            mWheelRotationSpeedFl.Update(payload.Wheel.WheelRotationSpeedFrontLeft.ToString("F2"));
            mWheelRotationSpeedFr.Update(payload.Wheel.WheelRotationSpeedFrontRight.ToString("F2"));
            mWheelRotationSpeedRl.Update(payload.Wheel.WheelRotationSpeedRearLeft.ToString("F2"));
            mWheelRotationSpeedRr.Update(payload.Wheel.WheelRotationSpeedRearRight.ToString("F2"));
            mWheelRotationDeltaF.Update((payload.Wheel.WheelRotationSpeedFrontLeft - payload.Wheel.WheelRotationSpeedFrontRight).ToString("F2"));
            mWheelRotationDeltaR.Update((payload.Wheel.WheelRotationSpeedRearLeft - payload.Wheel.WheelRotationSpeedRearRight).ToString("F2"));
        }

        public void ResetEvents()
        {
            mAcc.Update(string.Empty);
            mBrake.Update(string.Empty);
            mClutch.Update(string.Empty);
            mHandbrake.Update(string.Empty);
            mNormSuspTravelFl.Update(string.Empty);
            mNormSuspTravelFr.Update(string.Empty);
            mNormSuspTravelRl.Update(string.Empty);
            mNormSuspTravelRr.Update(string.Empty);


            mWheelPuddleFl.Update(string.Empty);
            mWheelPuddleFr.Update(string.Empty);
            mWheelPuddleRl.Update(string.Empty);
            mWheelPuddleRr.Update(string.Empty);

            mWheelRotationSpeedFl.Update(string.Empty);
            mWheelRotationSpeedFr.Update(string.Empty);
            mWheelRotationSpeedRl.Update(string.Empty);
            mWheelRotationSpeedRr.Update(string.Empty);
            mWheelRotationDeltaF.Update(string.Empty);
            mWheelRotationDeltaR.Update(string.Empty);
        }
    }
}
