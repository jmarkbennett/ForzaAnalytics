using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Models.Core
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public CarDetail CarDetail { get; set; }
        public TrackDetail TrackDetail { get; set; }
        public List<LapTime> LapTimes { get; set; }

        public Session()
        {
            SessionId = Guid.NewGuid();
            CarDetail = new CarDetail();
            TrackDetail = new TrackDetail();
            LapTimes = new List<LapTime>();
        }
    }
}
