using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class TyreData
    {
        private byte[] payload;

        public TyreData(ref byte[] payload)
        {
            this.payload = payload;
        }
        private float raw_tyreSlipRatioFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 84, 87); } }// F32 TyreSlipRatioFrontLeft;	4	84	87
        private float raw_tyreSlipRatioFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 88, 91); } }// F32 TyreSlipRatioFrontRight;	4	88	91
        private float raw_tyreSlipRatioRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 92, 95); } }// F32 TyreSlipRatioRearLeft;	4	92	95
        private float raw_tyreSlipRatioRearRight { get { return TelemetryHelper.extractSingle(ref payload, 96, 99); } }// F32 TyreSlipRatioRearRight;	4	96	99
        private float raw_tyreSlipAngleFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 164, 167); } }// F32 TyreSlipAngleFrontLeft;	4	164	167
        private float raw_tyreSlipAngleFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 168, 171); } }// F32 TyreSlipAngleFrontRight;	4	168	171
        private float raw_tyreSlipAngleRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 172, 175); } }// F32 TyreSlipAngleRearLeft;	4	172	175
        private float raw_tyreSlipAngleRearRight { get { return TelemetryHelper.extractSingle(ref payload, 176, 179); } }// F32 TyreSlipAngleRearRight;	4	176	179
        private float raw_tyreCombinedSlipFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 180, 183); } }// F32 TyreCombinedSlipFrontLeft;	4	180	183
        private float raw_tyreCombinedSlipFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 184, 187); } }// F32 TyreCombinedSlipFrontRight;	4	184	187
        private float raw_tyreCombinedSlipRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 188, 191); } }// F32 TyreCombinedSlipRearLeft;	4	188	191
        private float raw_tyreCombinedSlipRearRight { get { return TelemetryHelper.extractSingle(ref payload, 192, 195); } }// F32 TyreCombinedSlipRearRight;	4	192	195
        private float raw_tyreWearFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 311, 314); } }// F32 TyreWearFrontLeft;	4	311	314
        private float raw_tyreWearFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 315, 318); } }// F32 TyreWearFrontRight;	4	315	318
        private float raw_tyreWearRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 319, 322); } }// F32 TyreWearRearLeft;	4	319	322
        private float raw_tyreWearRearRight { get { return TelemetryHelper.extractSingle(ref payload, 323, 326); } }// F32 TyreWearRearRight;	4	323	326
        private float raw_tyreTempFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 256, 259); } }// F32 TyreTempFrontLeft;	4	256	259
        private float raw_tyreTempFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 260, 263); } }// F32 TyreTempFrontRight;	4	260	263
        private float raw_tyreTempRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 264, 267); } }// F32 TyreTempRearLeft;	4	264	267
        private float raw_tyreTempRearRight { get { return TelemetryHelper.extractSingle(ref payload, 268, 271); } }// F32 TyreTempRearRight;	4	268	271
        public float TyreSlipRatioFrontLeft { get { return raw_tyreCombinedSlipFrontLeft; } }
        public float TyreSlipRatioFrontRight { get { return raw_tyreCombinedSlipFrontRight; } }
        public float TyreSlipRatioRearLeft { get { return raw_tyreCombinedSlipRearLeft; } }
        public float TyreSlipRatioRearRight { get { return raw_tyreCombinedSlipRearRight; } }
        public float TyreWearFrontLeft { get { return (float)Math.Round(raw_tyreWearFrontLeft * 100.0, 2); } }
        public float TyreWearFrontRight { get { return (float)Math.Round(raw_tyreWearFrontRight * 100.0, 2); } }
        public float TyreWearRearLeft { get { return (float)Math.Round(raw_tyreWearRearLeft * 100.0, 2); } }
        public float TyreWearRearRight { get { return (float)Math.Round(raw_tyreWearRearRight * 100.0, 2); } }
        public float TyreWearRemainingFrontLeft { get { return 1 - raw_tyreWearFrontLeft; } }
        public float TyreWearRemainingFrontRight { get { return 1 - raw_tyreWearFrontRight; } }
        public float TyreWearRemainingRearLeft { get { return 1 - raw_tyreWearRearLeft; } }
        public float TyreWearRemainingRearRight { get { return 1 - raw_tyreWearRearRight; } }
        public double AvgTyreWear { get { return Math.Round((TyreWearFrontLeft + TyreWearFrontRight + TyreWearRearLeft + TyreWearRearRight) / 4.0f,2); } }

    }
}
