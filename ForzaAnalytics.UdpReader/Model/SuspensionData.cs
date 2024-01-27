using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class SuspensionData
    {
        private byte[] payload;

        public SuspensionData(ref byte[] payload)
        {
            this.payload = payload;
        }
        private float raw_normalizedSuspensionTravelFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 68, 71); } }// F32 NormalizedSuspensionTravelFrontLeft;	4	68	71
        private float raw_normalizedSuspensionTravelFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 72, 75); } }// F32 NormalizedSuspensionTravelFrontRight;	4	72	75
        private float raw_normalizedSuspensionTravelRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 76, 79); } }// F32 NormalizedSuspensionTravelRearLeft;	4	76	79
        private float raw_normalizedSuspensionTravelRearRight { get { return TelemetryHelper.extractSingle(ref payload, 80, 83); } }// F32 NormalizedSuspensionTravelRearRight;	4	80	83
        private float raw_suspensionTravelMetersFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 196, 199); } }// F32 SuspensionTravelMetersFrontLeft;	4	196	199
        private float raw_suspensionTravelMetersFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 200, 203); } }// F32 SuspensionTravelMetersFrontRight;	4	200	203
        private float raw_suspensionTravelMetersRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 204, 207); } }// F32 SuspensionTravelMetersRearLeft;	4	204	207
        private float raw_suspensionTravelMetersRearRight { get { return TelemetryHelper.extractSingle(ref payload, 208, 211); } }// F32 SuspensionTravelMetersRearRight;	4	208	211 

        public float NormalizedSuspensionTravelFrontLeft { get { return raw_normalizedSuspensionTravelFrontLeft; } }
        public float NormalizedSuspensionTravelFrontRight { get { return raw_normalizedSuspensionTravelFrontRight; } }
        public float NormalizedSuspensionTravelRearLeft { get { return raw_normalizedSuspensionTravelRearLeft; } }
        public float NormalizedSuspensionTravelRearRight { get { return raw_normalizedSuspensionTravelRearRight; } }
        public float SuspensionTravelMetersFrontLeft { get { return raw_suspensionTravelMetersFrontLeft; } }
        public float SuspensionTravelMetersFrontRight { get { return raw_suspensionTravelMetersFrontRight; } }
        public float SuspensionTravelMetersRearLeft { get { return raw_suspensionTravelMetersRearLeft; } }
        public float SuspensionTravelMetersRearRight { get { return raw_suspensionTravelMetersRearRight; } }
    }

}
