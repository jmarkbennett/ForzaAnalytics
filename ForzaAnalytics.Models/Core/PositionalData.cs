namespace ForzaAnalytics.Models.Core
{
    public class PositionalData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public PositionalData(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
