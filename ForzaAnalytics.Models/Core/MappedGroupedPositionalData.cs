using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Models.Core
{
    public class MappedGroupedPositionalData: GroupedPositionalData
    {
        public double MapScale { get; set; }
        public MappedGroupedPositionalData() {
            MapScale = 1;
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
                (position.X + XOffset) * (float)MapScale,
                position.Y,
                (position.Z + ZOffset) * (float)MapScale
            );
        }

        public PositionalData GetAdjustedPosition(int ordinal)
        {
            return new PositionalData(
                (Positions[ordinal].X + XOffset) * (float)MapScale,
                Positions[ordinal].Y,
                (Positions[ordinal].Z + ZOffset) * (float)MapScale
            );
        }
    }
}
