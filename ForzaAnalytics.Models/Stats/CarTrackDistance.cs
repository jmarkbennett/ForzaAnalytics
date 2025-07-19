using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Models.Stats
{
    public class CarTrackDistance
    {
        public string CarOrTrackName { get; set; }
        public double DistanceTravelled { get; set; }

        public string FormattedDistanceTravelled
        {
            get { return $"{DistanceTravelled.ToString("F2")}KM"; }
        }
    }
}
