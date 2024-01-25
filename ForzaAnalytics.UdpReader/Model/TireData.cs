using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class TireData
    {
        private byte[] payload;

        public TireData(ref byte[] payload)
        {
            this.payload = payload;
        }
        private float raw_tireSlipRatioFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 84, 87); } }// F32 TireSlipRatioFrontLeft;	4	84	87
        private float raw_tireSlipRatioFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 88, 91); } }// F32 TireSlipRatioFrontRight;	4	88	91
        private float raw_tireSlipRatioRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 92, 95); } }// F32 TireSlipRatioRearLeft;	4	92	95
        private float raw_tireSlipRatioRearRight { get { return TelemetryHelper.extractSingle(ref payload, 96, 99); } }// F32 TireSlipRatioRearRight;	4	96	99
        private float raw_tireSlipAngleFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 164, 167); } }// F32 TireSlipAngleFrontLeft;	4	164	167
        private float raw_tireSlipAngleFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 168, 171); } }// F32 TireSlipAngleFrontRight;	4	168	171
        private float raw_tireSlipAngleRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 172, 175); } }// F32 TireSlipAngleRearLeft;	4	172	175
        private float raw_tireSlipAngleRearRight { get { return TelemetryHelper.extractSingle(ref payload, 176, 179); } }// F32 TireSlipAngleRearRight;	4	176	179
        private float raw_tireCombinedSlipFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 180, 183); } }// F32 TireCombinedSlipFrontLeft;	4	180	183
        private float raw_tireCombinedSlipFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 184, 187); } }// F32 TireCombinedSlipFrontRight;	4	184	187
        private float raw_tireCombinedSlipRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 188, 191); } }// F32 TireCombinedSlipRearLeft;	4	188	191
        private float raw_tireCombinedSlipRearRight { get { return TelemetryHelper.extractSingle(ref payload, 192, 195); } }// F32 TireCombinedSlipRearRight;	4	192	195
        private float raw_tireWearFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 311, 314); } }// F32 TireWearFrontLeft;	4	311	314
        private float raw_tireWearFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 315, 318); } }// F32 TireWearFrontRight;	4	315	318
        private float raw_tireWearRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 319, 322); } }// F32 TireWearRearLeft;	4	319	322
        private float raw_tireWearRearRight { get { return TelemetryHelper.extractSingle(ref payload, 323, 326); } }// F32 TireWearRearRight;	4	323	326
        private float raw_tireTempFrontLeft { get { return TelemetryHelper.extractSingle(ref payload, 256, 259); } }// F32 TireTempFrontLeft;	4	256	259
        private float raw_tireTempFrontRight { get { return TelemetryHelper.extractSingle(ref payload, 260, 263); } }// F32 TireTempFrontRight;	4	260	263
        private float raw_tireTempRearLeft { get { return TelemetryHelper.extractSingle(ref payload, 264, 267); } }// F32 TireTempRearLeft;	4	264	267
        private float raw_tireTempRearRight { get { return TelemetryHelper.extractSingle(ref payload, 268, 271); } }// F32 TireTempRearRight;	4	268	271

    }
}
