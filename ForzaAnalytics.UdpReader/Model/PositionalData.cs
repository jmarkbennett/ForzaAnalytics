using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class PositionalData
    {
        private byte[] payload;

        public PositionalData(ref byte[] payload)
        {
            this.payload = payload;
        }
        #region Private Properties
        private float raw_accelerationX { get { return TelemetryHelper.extractSingle(ref payload, 20, 23); } }// F32 AccelerationX;	4	20	23
        private float raw_accelerationY { get { return TelemetryHelper.extractSingle(ref payload, 24, 27); } }// F32 AccelerationY;	4	24	27
        private float raw_accelerationZ { get { return TelemetryHelper.extractSingle(ref payload, 28, 31); } } // F32 AccelerationZ;	4	28	31
        private float raw_velocityX { get { return TelemetryHelper.extractSingle(ref payload, 32, 35); } }// F32 VelocityX;	4	32	35
        private float raw_velocityY { get { return TelemetryHelper.extractSingle(ref payload, 36, 39); } }// F32 VelocityY;	4	36	39
        private float raw_velocityZ { get { return TelemetryHelper.extractSingle(ref payload, 40, 43); } } // F32 VelocityZ;	4	40	43
        private float raw_angularVelocityX { get { return TelemetryHelper.extractSingle(ref payload, 44, 47); } }// F32 AngularVelocityX;	4	44	47
        private float raw_angularVelocityY { get { return TelemetryHelper.extractSingle(ref payload, 48, 51); } }// F32 AngularVelocityY;	4	48	51
        private float raw_angularVelocityZ { get { return TelemetryHelper.extractSingle(ref payload, 52, 55); } }// F32 AngularVelocityZ;	4	52	55
        private float raw_positionX { get { return TelemetryHelper.extractSingle(ref payload, 232, 235); } }// F32 PositionX;	4	232	235
        private float raw_positionY { get { return TelemetryHelper.extractSingle(ref payload, 236, 239); } }// F32 PositionY;	4	236	239
        private float raw_positionZ { get { return TelemetryHelper.extractSingle(ref payload, 240, 243); } }// F32 PositionZ;	4	240	243

        #endregion
        public float PositionX { get { return raw_positionX; } }
        public float PositionY { get { return raw_positionY; } }   
        public float PositionZ { get { return raw_positionZ; } }
        public float AccelerationX { get { return raw_accelerationX; } }
        public float AccelerationY { get { return raw_accelerationY; } }
        public float AccelerationZ { get { return raw_accelerationZ; } }    

    }
}
