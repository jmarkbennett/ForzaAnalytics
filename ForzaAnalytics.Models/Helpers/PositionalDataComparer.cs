using ForzaAnalytics.Models.Core;


namespace ForzaAnalytics.Models.Helpers
{
    public class PositionalDataComparer: IEqualityComparer<PositionalData>
    {
        public bool Equals(PositionalData? x, PositionalData? y) {
        return x?.X == y?.X && x?.Y == y?.Y && x?.Z == y?.Z;
        }

        public int GetHashCode(PositionalData obj)
        {
            return HashCode.Combine(obj.X, obj.Y, obj.Z);
        }
    }
}
