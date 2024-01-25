using ForzaAnalytics.UdpReader.Helper;

namespace ForzaAnalytics.UdpReader.Model
{
    public class Telemetry
    {
        private byte[] payload;

        private WheelData wheel;
        private CarDetail car;
        private RaceData race;
        private TireData tire;
        private SuspensionData suspension;
        private PositionalData position;

        public Telemetry(byte[] payload)
        {
            this.payload = payload;
            wheel = new WheelData(ref payload);
            car = new CarDetail(ref payload);
            race = new RaceData(ref payload);
            tire = new TireData(ref payload);
            suspension = new SuspensionData(ref payload);
            position = new PositionalData(ref payload);

        }

        #region Raw Values (Should be private)
        private int raw_isRaceOn { get { return TelemetryHelper.extractInt32Value(ref payload, 0, 3); } } // S32 IsRaceOn;	4	0	3
        private int raw_acceleration { get { return (int)payload[303]; } } // % Applied, // U8 Accel;	1	303	303
        private int raw_brake { get { return (int)payload[304]; } } // % Applied // U8 Brake;	1	304	304
        private int raw_clutch { get { return (int)payload[305]; } } // % Applied // U8 Clutch;	1	305	305
        private int raw_handbrake { get { return (int)payload[306]; } }// % Applied // U8 HandBrake;	1	306	306
        private int raw_gear { get { return (int)payload[307]; } } // U8 Gear;	1	307	307     
        private float raw_speed { get { return TelemetryHelper.extractSingle(ref payload, 244, 247); } } // F32 Speed;	4	244	247
        private float raw_power { get { return TelemetryHelper.extractSingle(ref payload, 248, 251); } } //F32 Power;	4	248	251
        private float raw_rpm { get { return TelemetryHelper.extractSingle(ref payload, 16, 19); } } // F32 CurrentEngineRpm;	4	16	19    
        private float raw_engineMaxRpm { get { return TelemetryHelper.extractSingle(ref payload, 8, 11); } } // F32 EngineMaxRpm;	4	8	11
        private float raw_engineIdleRpm { get { return TelemetryHelper.extractSingle(ref payload, 12, 15); } } // F32 EngineIdleRpm;	4	12	15                                              
        private float raw_yaw { get { return TelemetryHelper.extractSingle(ref payload, 56, 59); } }// F32 Yaw;	4	56	59
        private float raw_pitch { get { return TelemetryHelper.extractSingle(ref payload, 60, 63); } }// F32 Pitch;	4	60	63
        private float raw_roll { get { return TelemetryHelper.extractSingle(ref payload, 64, 67); } }// F32 Roll;	4	64	67
        private float raw_torque { get { return TelemetryHelper.extractSingle(ref payload, 252, 255); } }// F32 Torque;	4	252	255
        private float raw_boost { get { return TelemetryHelper.extractSingle(ref payload, 272, 275); } }// F32 Boost;	4	272	275
        private float raw_fuel { get { return TelemetryHelper.extractSingle(ref payload, 276, 279); } }// F32 Fuel;	4	276	279
        private float raw_distanceTraveled { get { return TelemetryHelper.extractSingle(ref payload, 280, 283); } }// F32 DistanceTraveled;	4	280	283
        private int raw_steeringAngle { get { return (int)payload[308]; } }// S8 Steer;	1	308	308
        private int raw_normalizedDrivingLine { get { return (int)payload[309]; } }// S8 NormalizedDrivingLine;	1	309	309
        private int raw_normalizedAIBrakeDifference { get { return (int)payload[310]; } }// S8 NormalizedAIBrakeDifference;	1	310	310

        #endregion

        public WheelData Wheel { get { return wheel; } }
        public CarDetail Car { get { return car; } }
        public RaceData Race { get { return race; } }
        public TireData Tire { get { return tire; } }
        public SuspensionData Suspension { get { return suspension; } }
        public PositionalData Position { get { return position; } }

        public string IsRaceOn
        {
            get { if (raw_isRaceOn == 0) return "No"; else return "Yes"; }
        }
        public double Acceleration
        {
            get
            {
                return Math.Round(
                                    (raw_acceleration / 255.0) * 100.0, 3);
            }
        }
        public double Brake
        {
            get
            {
                return Math.Round(
                                    (raw_brake / 255.0) * 100.0, 3);
            }
        }
        public double Handbrake
        {
            get
            {
                return Math.Round(
                                    (raw_handbrake / 255.0) * 100.0, 3);
            }
        }
        public double Clutch
        {
            get
            {
                return Math.Round(
                                    (raw_clutch / 255.0) * 100.0, 3);
            }
        }
        public double Speed_Mph
        {
            get
            {
                return Math.Round(raw_speed * 2.23694, 2);
            }
        }
        public double Speed_Kph
        {
            get
            {
                return Math.Round(raw_speed * 3.6, 2);
            }
        }
        public bool isReportingActive { get { return raw_isRaceOn > 0; } }
        public float DistanceTravelled { get { return raw_distanceTraveled; } }
        public string GearNumber { get { return raw_gear == 11 ? "N" : raw_gear == 0 ? "R" : raw_gear.ToString(); } }
        public float EngineRpm { get { return raw_rpm; } }
        public float EngineMaxRpm { get { return raw_engineMaxRpm; } }
        public float EnginePowerKw { get { return raw_power; } }
        public float Fuel { get { return raw_fuel; } }
        public float Boost { get { return raw_boost; } }
        public int SteeringAngle { get { return raw_steeringAngle; } }
         public float Yaw { get { return raw_yaw;} }
        public float Pitch { get { return raw_pitch; } }
        public float Roll { get { return raw_roll; } }
        public float TorqueNm { get { return raw_torque; } }
        public float TorqueOneFootPound { get { return (raw_torque / (float)1.356); } }
        public float EnginePowerHp { get { return ((raw_power * (float)1.341) / 1000); } }
    }
}