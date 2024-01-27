namespace ForzaAnalytics.Models.Core
{
    public class GroupedExtendedPositionalData
    {
        public List<ExtendedPositionalData> ExtendedPositions { get; set; }
        public int XOffset { get; set; }
        public int ZOffset { get; set; }

        public GroupedExtendedPositionalData()
        {
            ExtendedPositions = new List<ExtendedPositionalData>();
            XOffset = 0;
            ZOffset = 0;
        }
        public List<ExtendedPositionalData> GetAdjustedPositions()
        {
            var result = new List<ExtendedPositionalData>();
            foreach (var position in ExtendedPositions)
                result.Add(new ExtendedPositionalData(position.X + XOffset, position.Y, position.Z + ZOffset) { 
                    Acceleration = position.Acceleration,
                    Brake = position.Brake,
                    Clutch = position.Clutch,
                    Handbrake = position.Handbrake,
                    Speed_Mph = position.Speed_Mph,
                    LapNumber = position.LapNumber,
                    Speed_Mps = position.Speed_Mps,
                    LapTime = position.LapTime,
                    RaceTime = position.RaceTime,
                    GearNumber= position.GearNumber
                });
            return result;
        }
        public void ResetPositions()
        {
            ExtendedPositions = new List<ExtendedPositionalData>();
        }
    }
}
