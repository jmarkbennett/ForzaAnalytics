namespace ForzaAnalytics.Models.Core
{
    public class GroupedPositionalData
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; }
        public List<PositionalData> Positions { get; set; }
        public int XOffset { get; set; }
        public int ZOffset { get; set; }

        public GroupedPositionalData()
        {
            TrackId = 0;
            TrackName = string.Empty;
            Positions = new List<PositionalData>();
        }

        public List<PositionalData> GetAdjustedPositions()
        {
            var result = new List<PositionalData>();
            foreach(var position in Positions)
                result.Add(new PositionalData(position.X + XOffset, position.Y, position.Z + ZOffset));
            return result;
        }

        public void ResetPositions()
        {
            Positions = new List<PositionalData>();
        }
    }
}
