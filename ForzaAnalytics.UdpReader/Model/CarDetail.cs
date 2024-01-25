using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class CarDetail
    {
        private byte[] payload;

        public CarDetail(ref byte[] payload)
        {
            this.payload = payload;
        }
        private int raw_cpi { get { return TelemetryHelper.extractInt32Value(ref payload, 220, 223); } } // S32 CarPerformanceIndex;	4	220	223
        private int raw_carClass { get { return TelemetryHelper.extractInt32Value(ref payload, 216, 219); } } // create Enum, S32 CarClass;	4	216	219
        private int raw_carOrdinal { get { return TelemetryHelper.extractInt32Value(ref payload, 212, 215); } } // S32 CarOrdinal;	4	212	215. car ID, needs to be looked up against a database (or perhaps the files)?
        private int raw_drivetrainType { get { return TelemetryHelper.extractInt32Value(ref payload, 224, 227); } }// S32 DrivetrainType;	4	224	227
        private int raw_numCylinders { get { return TelemetryHelper.extractInt32Value(ref payload, 228, 231); } }// S32 NumCylinders;	4	228	231

        public string CarPerformanceIndex { get { return raw_cpi.ToString(); } }
        public int CarClass { get { return raw_carClass; } }
        public int CarIdentifier { get { return raw_carOrdinal; } }
        public int DrivetrainType { get { return raw_drivetrainType; } }
        public int NumberOfCylinders { get {  return raw_numCylinders;} }

    }
}
