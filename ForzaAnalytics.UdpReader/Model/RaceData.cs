using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class RaceData
    {
        private byte[] payload;

        public RaceData(ref byte[] payload)
        {
            this.payload = payload;
        }
        #region Private Properties
        private uint raw_lapNumber { get { return TelemetryHelper.extractUInt16Value(ref payload, 300, 301); } } // U16 LapNumber;	2	300	301
        private int raw_racePosition { get { return (int)payload[302]; } } // U8 RacePosition;	1	302	302
        private uint raw_timestampMs { get { return TelemetryHelper.extractUInt32Value(ref payload, 4, 7); } } // U32 TimestampMS;	4	4	7
        private float raw_bestLap { get { return TelemetryHelper.extractSingle(ref payload, 284, 287); } }// F32 BestLap;	4	284	287
        private float raw_lastLap { get { return TelemetryHelper.extractSingle(ref payload, 288, 291); } }// F32 LastLap;	4	288	291
        private float raw_currentLap { get { return TelemetryHelper.extractSingle(ref payload, 292, 295); } }// F32 CurrentLap;	4	292	295
        private float raw_currentRaceTime { get { return TelemetryHelper.extractSingle(ref payload, 296, 299); } }// F32 CurrentRaceTime;	4	296	299
        private int raw_trackOrdinal { get { return TelemetryHelper.extractInt32Value(ref payload, 327, 330); } }// S32 TrackOrdinal;	4	327	330

        #endregion

        public int LapNumber { get { return (int)raw_lapNumber; } }
        public int RacePosition { get { return raw_racePosition;} }
        public float BestLapTime { get { return raw_bestLap; } }
        public float LastLapTime { get { return raw_lastLap; } }
        public float CurrentLapTime { get { return raw_currentLap; } }
        public float CurrentRaceTime { get { return raw_currentRaceTime; } }
        public int TrackIdentifier { get { return raw_trackOrdinal; } } 
    }
}
