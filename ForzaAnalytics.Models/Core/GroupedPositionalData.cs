using static System.Formats.Asn1.AsnWriter;

namespace ForzaAnalytics.Models.Core
{
    public class GroupedPositionalData
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; }
        public List<PositionalData> Positions { get; set; }
        public int XOffset { get; set; }
        public int ZOffset { get; set; }
        public double MapScale { get; set; }
        public bool IsRotated { get; set; }

        public GroupedPositionalData()
        {
            TrackId = 0;
            TrackName = string.Empty;
            Positions = new List<PositionalData>();
            MapScale = 1;
        }
        public void ResetPositions()
        {
            Positions = new List<PositionalData>();
        }
        public List<PositionalData> GetAdjustedPositions()
        {
            var result = new List<PositionalData>();
            foreach (var position in Positions)
                result.Add(GetAdjustedPosition(position));
            return result;
        }
        public PositionalData GetAdjustedPosition(PositionalData position)
        {
            return new PositionalData(
                GetAdjustedXCoordinate(position.X, position.Z),
                position.Y,
                GetAdjustedZCoordinate(position.X, position.Z)
            );
        }
        public PositionalData GetAdjustedPosition(int ordinal)
        {
            return new PositionalData(
                GetAdjustedXCoordinate(Positions[ordinal].X, Positions[ordinal].Z),
                Positions[ordinal].Y,
                GetAdjustedZCoordinate(Positions[ordinal].X, Positions[ordinal].Z)
            );
        }
        private float GetAdjustedXCoordinate(float x,float z)
        {
            if (IsRotated)
                return (z + ZOffset) * (float)MapScale;
            else
                return (x + XOffset) * (float)MapScale;
        }
        private float GetAdjustedZCoordinate(float x,float z)
        {
            if (IsRotated)
                return (x + XOffset) * (float)MapScale;
            else
                return (z + ZOffset) * (float)MapScale;
        }
    }
}