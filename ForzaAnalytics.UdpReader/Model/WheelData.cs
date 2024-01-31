using ForzaAnalytics.UdpReader.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.UdpReader.Model
{
    public class WheelData
    {
        private byte[] payload;

        public WheelData(ref byte[] payload)
        {
            this.payload = payload;
        }
        private float raw_wheelRotationSpeedFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 100, 103); } }// F32 WheelRotationSpeedFrontLeft;	4	100	103
        private float raw_wheelRotationSpeedFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 104, 107); } }// F32 WheelRotationSpeedFrontRight;	4	104	107
        private float raw_wheelRotationSpeedRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 108, 111); } }// F32 WheelRotationSpeedRearLeft;	4	108	111
        private float raw_wheelRotationSpeedRearRight { get { return TelemetryHelper.extractSingle(ref payload, 112, 115); } }// F32 WheelRotationSpeedRearRight;	4	112	115
        private int raw_wheelOnRumbleStripFrontLeft { get { return TelemetryHelper.extractInt32Value(ref payload, 116, 119); } }// S32 WheelOnRumbleStripFrontLeft;	4	116	119
        private int raw_wheelOnRumbleStripFrontRight { get { return TelemetryHelper.extractInt32Value(ref payload, 120, 123); } }// S32 WheelOnRumbleStripFrontRight;	4	120	123
        private int raw_wheelOnRumbleStripRearLeft { get { return TelemetryHelper.extractInt32Value(ref payload, 124, 127); } }// S32 WheelOnRumbleStripRearLeft;	4	124	127
        private int raw_wheelOnRumbleStripRearRight { get { return TelemetryHelper.extractInt32Value(ref payload, 128, 131); } }// S32 heelOnRumbleStripRearRight;	4	128	131
        private float raw_wheelInPuddleDepthFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 132, 135); } }// F32 WheelInPuddleDepthFrontLeft;	4	132	135
        private float raw_wheelInPuddleDepthFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 136, 139); } }// F32 WheelInPuddleDepthFrontRight;	4	136	139
        private float raw_wheelInPuddleDepthRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 140, 143); } }// F32 WheelInPuddleDepthRearLeft;	4	140	143
        private float raw_wheelInPuddleDepthRearRight { get { return TelemetryHelper.extractSingle(ref payload, 144, 147); } }// F32 WheelInPuddleDepthRearRight;	4	144	147
        private float raw_surfaceRumbleFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 148, 151); } }// F32 SurfaceRumbleFrontLeft;	4	148	151
        private float raw_surfaceRumbleFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 152, 155); } }// F32 SurfaceRumbleFrontRight;	4	152	155
        private float raw_surfaceRumbleRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 156, 159); } }// F32 SurfaceRumbleRearLeft;	4	156	159
        private float raw_surfaceRumbleRearRight { get { return TelemetryHelper.extractSingle(ref payload, 160, 163); } }// F32 SurfaceRumbleRearRight;	4	160	163


        public float WheelRotationSpeedFrontLeft { get { return raw_wheelRotationSpeedFrontLeft; } }
        public float WheelRotationSpeedFrontRight { get { return raw_wheelRotationSpeedFrontRight; } }
        public float WheelRotationSpeedFrontDelta { get { return WheelRotationSpeedFrontLeft - WheelRotationSpeedFrontRight; } }
        public float WheelRotationSpeedRearLeft { get { return raw_wheelRotationSpeedRearLeft; } }
        public float WheelRotationSpeedRearRight { get { return raw_wheelRotationSpeedRearRight; } }
        public float WheelRotationSpeedRearDelta { get { return WheelRotationSpeedRearLeft - WheelRotationSpeedRearRight; } }
        public float WheelInPuddleDepthFrontLeft { get { return raw_wheelInPuddleDepthFrontLeft; } }
        public float WheelInPuddleDepthFrontRight { get { return raw_wheelInPuddleDepthFrontRight; } }
        public float WheelInPuddleDepthRearLeft { get { return raw_wheelInPuddleDepthRearLeft; } }
        public float WheelInPuddleDepthRearRight { get { return raw_wheelInPuddleDepthRearRight; } }


    }
}
