using ForzaAnalytics.UdpReader.Model;

namespace ForzaAnalytics.Models.Core
{
    public class GroupedExtendedPositionalData
    {
        public List<ExtendedPositionalData> ExtendedPositions { get; set; }
        public int XOffset { get; set; }
        public int ZOffset { get; set; }
        public double MapScale { get; set; }
        public bool IsRotated { get; set; }
        public int TrackId { get; set; }
        public GroupedExtendedPositionalData()
        {
            ExtendedPositions = new List<ExtendedPositionalData>();
            XOffset = 0;
            ZOffset = 0;
            MapScale = 1;
            IsRotated = false;
            TrackId = -1;
        }
        public List<ExtendedPositionalData> GetAdjustedPositions()
        {
            var result = new List<ExtendedPositionalData>();
            foreach (var position in ExtendedPositions)
            {
                var current = GetAdjustedPosition(position);
                result.Add(current);
            }
            return result;
        }
        public void ResetPositions()
        {
            ExtendedPositions = new List<ExtendedPositionalData>();
        }
        public ExtendedPositionalData GetAdjustedPosition(ExtendedPositionalData position)
        {
            var result = new ExtendedPositionalData(
                GetAdjustedXCoordinate(position.X, position.Z),
                position.Y,
                GetAdjustedZCoordinate(position.X, position.Z)
            );
            result.Acceleration = position.Acceleration;
            result.Brake = position.Brake;
            result.Clutch = position.Clutch;
            result.Handbrake = position.Handbrake;
            result.Speed_Mph = position.Speed_Mph;
            result.LapNumber = position.LapNumber;
            result.Speed_Mps = position.Speed_Mps;
            result.LapTime = position.LapTime;
            result.RaceTime = position.RaceTime;
            result.GearNumber = position.GearNumber;
            result.FuelRemaining = position.FuelRemaining;
            result.AvgTireWear = position.AvgTireWear;
            return result;
        }
        public ExtendedPositionalData GetAdjustedPosition(int ordinal)
        {
            var result = new ExtendedPositionalData(
                GetAdjustedXCoordinate(ExtendedPositions[ordinal].X, ExtendedPositions[ordinal].Z),
                ExtendedPositions[ordinal].Y,
                GetAdjustedZCoordinate(ExtendedPositions[ordinal].X, ExtendedPositions[ordinal].Z)
            );
            result.Acceleration = ExtendedPositions[ordinal].Acceleration;
            result.Brake = ExtendedPositions[ordinal].Brake;
            result.Clutch = ExtendedPositions[ordinal].Clutch;
            result.Handbrake = ExtendedPositions[ordinal].Handbrake;
            result.Speed_Mph = ExtendedPositions[ordinal].Speed_Mph;
            result.LapNumber = ExtendedPositions[ordinal].LapNumber;
            result.Speed_Mps = ExtendedPositions[ordinal].Speed_Mps;
            result.LapTime = ExtendedPositions[ordinal].LapTime;
            result.RaceTime = ExtendedPositions[ordinal].RaceTime;
            result.GearNumber = ExtendedPositions[ordinal].GearNumber;
            result.FuelRemaining = ExtendedPositions[ordinal].FuelRemaining;
            result.AvgTireWear = ExtendedPositions[ordinal].AvgTireWear;
            return result;
        }

        public PositionalData GetAdjustedPosition(Telemetry position)
        {
            var result = new PositionalData(
                GetAdjustedXCoordinate(position.Position.PositionX, position.Position.PositionZ),
                position.Position.PositionY,
                GetAdjustedZCoordinate(position.Position.PositionX, position.Position.PositionZ)
            );

            return result;
        }
        public float GetAdjustedXCoordinate(float x, float z)
        {
            if (IsRotated)
                return (z + ZOffset) * (float)MapScale;
            else
                return (x + XOffset) * (float)MapScale;
        }
        public float GetAdjustedZCoordinate(float x, float z)
        {
            if (IsRotated)
                return (x + XOffset) * (float)MapScale;
            else
                return (z + ZOffset) * (float)MapScale;
        }
    }
}
