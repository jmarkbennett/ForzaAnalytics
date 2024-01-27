using ForzaAnalytics.Controls;
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
            mAcc.Update(payload.Acceleration.ToString());
            mBrake.Update(payload.Brake.ToString());
            mClutch.Update(payload.Clutch.ToString());
            mHandbrake.Update(payload.Handbrake.ToString());
            mNormSuspTravelFl.Update(payload.Suspension.NormalizedSuspensionTravelFrontLeft.ToString());
            mNormSuspTravelFr.Update(payload.Suspension.NormalizedSuspensionTravelFrontRight.ToString());
            mNormSuspTravelRl.Update(payload.Suspension.NormalizedSuspensionTravelRearLeft.ToString());
            mNormSuspTravelRr.Update(payload.Suspension.NormalizedSuspensionTravelRearRight.ToString());

            mSuspTravelFl.Update(payload.Suspension.SuspensionTravelMetersFrontLeft.ToString());
            mSuspTravelFr.Update(payload.Suspension.SuspensionTravelMetersFrontRight.ToString());
            mSuspTravelRl.Update(payload.Suspension.SuspensionTravelMetersRearLeft.ToString());
            mSuspTravelRr.Update(payload.Suspension.SuspensionTravelMetersRearRight.ToString());

            mWheelPuddleFl.Update(payload.Wheel.WheelInPuddleDepthFrontLeft.ToString());
            mWheelPuddleFr.Update(payload.Wheel.WheelInPuddleDepthFrontRight.ToString());
            mWheelPuddleRl.Update(payload.Wheel.WheelInPuddleDepthRearLeft.ToString());
            mWheelPuddleRr.Update(payload.Wheel.WheelInPuddleDepthRearRight.ToString());

            mWheelRotationSpeedFl.Update(payload.Wheel.WheelRotationSpeedFrontLeft.ToString());
            mWheelRotationSpeedFr.Update(payload.Wheel.WheelRotationSpeedFrontRight.ToString());
            mWheelRotationSpeedRl.Update(payload.Wheel.WheelRotationSpeedRearLeft.ToString());
            mWheelRotationSpeedRr.Update(payload.Wheel.WheelRotationSpeedRearRight.ToString());
        }
    }
}
