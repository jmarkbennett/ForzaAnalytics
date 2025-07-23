using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Models.Stats
{
    public class CarTrackDuration
    {
        public string CarOrTrackName { get; set; }
        public string SecondaryDetail { get; set; }
        public TimeSpan Duration { get; set; }

        public string FormattedDuration
        {
            get { return $"{Duration.ToString(@"hh\:mm\:ss")}"; }
        }
    }
}
