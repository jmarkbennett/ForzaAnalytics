using ForzaAnalytics.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Models.Service
{
    public class LapDetail
    {

        private List<double> currentSpeeds;
        private List<double> currentAccelerations;
        private List<double> currentBrakeApplied;
        private List<double> fuelConsumption;
        private List<int> currentCoasting;
        private int currentLapNumber;
        private float initialfuel = -1;

        public float InitialFuel { get { return initialfuel; } set { initialfuel = value; } }
        public int CurrentLapNumber { get { return currentLapNumber; } set { currentLapNumber = value; } }
        public ObservableCollection<LapTime> LapTimes { get; set; }

        public LapDetail()
        {
            LapTimes = [];
            currentSpeeds = [];
            currentAccelerations = [];
            currentBrakeApplied = [];
            currentCoasting = [];
            fuelConsumption = [];
            currentLapNumber = 0;
        }
        public double AverageSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Average() : 0.0; } }
        public double PercentBrakeApplied { get { return ((double)currentBrakeApplied.Count(x => x > 0) / (double)currentBrakeApplied.Count()); } }
        public double PercentFullThrottle { get { return ((double)currentAccelerations.Count(x => x == 100) / (double)currentBrakeApplied.Count()); } }
        public double PercentCoasting { get { return ((double)currentCoasting.Count() / (double)currentAccelerations.Count()); } }
        public double MaxSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Max() : 0.0; } }
        public double MinSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Min() : 0.0; } }
        public double FuelUsed { get { return fuelConsumption.Any() ? fuelConsumption.Max() - fuelConsumption.Min() : 0; } }
    }
}
