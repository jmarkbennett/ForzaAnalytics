namespace ForzaAnalytics.Models.Core
{
    public class ExtendedPositionalData : PositionalData
    {
        public ExtendedPositionalData(float x, float y, float z): base(x,y,z) {
            GearNumber = string.Empty;
        }
        public double Speed_Mps { get; set; }
        public double Speed_Mph { get; set; }
        public double Brake { get; set; }
        public double Acceleration { get; set; }
        public double Clutch { get; set; }
        public double Handbrake { get; set; }
        public float RaceTime { get; set; }
        public float LapTime { get; set; }
        public int LapNumber { get; set; }
        public string GearNumber { get; set; }
        public double FuelRemaining { get; set; }
        public float AvgTireWear { get; set; }
    }
}
