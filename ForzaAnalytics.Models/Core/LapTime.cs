using System.ComponentModel;
using ForzaAnalytics.Models.Formatters;
namespace ForzaAnalytics.Models.Core
{
    public class LapTime : INotifyPropertyChanged
    {
        private float timeInSeconds;
        private DateTime timeOfLapTime;
        private int lapNumber;
        private bool isBestLap;
        private double distanceTravelled;
        private double totalDistanceTravelled;
        private double averageSpeed;
        private double fuelRemaining;
        private double avgTyreWear;
        private double flTyreWear;
        private double frTyreWear;
        private double rlTyreWear;
        private double rrTyreWear;
        private double percentFullThrottle;
        private double percentBrakeApplied;
        private double minSpeed;
        private double maxSpeed;
        private double fuelUsed;
        private double percentCoasting;
        private int positionChanges;
        private int lapStartingPosition;
        private int racePosition;
        private string sessionSummary;
        private Guid sessionId;
        public Guid SessionId
        {
            get { return sessionId; }
            set
            {
                sessionId = value;
                OnPropertyChanged(nameof(SessionId));
            }
        }
        public float TimeInSeconds
        {
            get { return timeInSeconds; }
            set
            {
                timeInSeconds = value;
                OnPropertyChanged(nameof(TimeInSeconds));
            }
        }
        public DateTime TimeOfLapTime
        {
            get { return timeOfLapTime; }
            set
            {
                timeOfLapTime = value;
                OnPropertyChanged(nameof(TimeOfLapTime));
            }
        }
        public int LapNumber
        {
            get { return lapNumber; }
            set
            {
                lapNumber = value;
                OnPropertyChanged(nameof(LapNumber));
            }
        }
        public string SessionSummary
        {
            get { return sessionSummary; }
            set
            {
                sessionSummary = value;
                OnPropertyChanged(nameof(SessionSummary));
            }
        }
        public int RacePosition
        {
            get { return racePosition; }
            set
            {
                racePosition = value;
                OnPropertyChanged(nameof(RacePosition));
            }
        }
        public int LapStartingPosition
        {
            get { return lapStartingPosition; }
            set
            {
                lapStartingPosition = value;
                OnPropertyChanged(nameof(LapStartingPosition));
            }
        }
        public bool IsBestLap
        {
            get { return isBestLap; }
            set
            {
                isBestLap = value;
                OnPropertyChanged(nameof(IsBestLap));
            }
        }
        public double AverageSpeed
        {
            get { return averageSpeed; }
            set
            {
                averageSpeed = value;
                OnPropertyChanged(nameof(AverageSpeed));
            }
        }
        public double FuelRemaining
        {
            get { return fuelRemaining; }
            set
            {
                fuelRemaining = value;
                OnPropertyChanged(nameof(FuelRemaining));
            }
        }
        public double AvgTyreWear
        {
            get { return avgTyreWear; }
            set
            {
                avgTyreWear = value;
                OnPropertyChanged(nameof(AvgTyreWear));
            }
        }
        public double PercentFullThrottle
        {
            get { return percentFullThrottle; }
            set
            {
                percentFullThrottle = value;
                OnPropertyChanged(nameof(PercentFullThrottle));
            }
        }
        public double PercentBrakeApplied
        {
            get { return percentBrakeApplied; }
            set
            {
                percentBrakeApplied = value;
                OnPropertyChanged(nameof(PercentBrakeApplied));
            }
        }
        public double PercentCoasting
        {
            get { return percentCoasting; }
            set
            {
                percentCoasting = value;
                OnPropertyChanged(nameof(PercentCoasting));
            }
        }
        public double MaxSpeed
        {
            get { return maxSpeed; }
            set
            {
                maxSpeed = value;
                OnPropertyChanged(nameof(MaxSpeed));
            }
        }
        public double MinSpeed
        {
            get { return minSpeed; }
            set
            {
                minSpeed = value;
                OnPropertyChanged(nameof(MinSpeed));
            }
        }
        public double FuelUsed
        {
            get { return fuelUsed; }
            set
            {
                fuelUsed = value;
                OnPropertyChanged(nameof(FuelUsed));
            }
        }
        public double FlTyreWear
        {
            get { return flTyreWear; }
            set
            {
                flTyreWear = value;
                OnPropertyChanged(nameof(FlTyreWear));
            }
        }
        public double FrTyreWear
        {
            get { return frTyreWear; }
            set
            {
                frTyreWear = value;
                OnPropertyChanged(nameof(FrTyreWear));
            }
        }
        public double RlTyreWear
        {
            get { return rlTyreWear; }
            set
            {
                rlTyreWear = value;
                OnPropertyChanged(nameof(RlTyreWear));
            }
        }
        public double RrTyreWear
        {
            get { return rrTyreWear; }
            set
            {
                rrTyreWear = value;
                OnPropertyChanged(nameof(RrTyreWear));
            }
        }
        public int PositionChanges
        {
            get { return positionChanges; }
            set
            {
                positionChanges = value;
                OnPropertyChanged(nameof(PositionChanges));
            }
        }
        public double DistanceTravelled
        {
            get { return distanceTravelled; }
            set
            {
                distanceTravelled = value;
                OnPropertyChanged(nameof(DistanceTravelled));
            }
        }
        public double TotalDistanceTravelled
        {
            get { return totalDistanceTravelled; }
            set
            {
                totalDistanceTravelled = value;
                OnPropertyChanged(nameof(TotalDistanceTravelled));
            }
        }
        public string FormattedLapTime
        {
            get { return Formatting.FormattedTime(timeInSeconds); }
        }
        public string FormattedAverageSpeed
        {
            get { return $"{averageSpeed.ToString("F2")}MPH"; }
        }
        public string FormattedMinSpeed
        {
            get { return $"{minSpeed.ToString("F2")}MPH"; }
        }
        public string FormattedMaxSpeed
        {
            get { return $"{maxSpeed.ToString("F2")}MPH"; }
        }
        public string FormattedPercentBrakeApplied
        {
            get { return Formatting.FormattedPercentage(percentBrakeApplied); }
        }
        public string FormattedPercentFullThrottle
        {
            get { return Formatting.FormattedPercentage(percentFullThrottle); }
        }
        public string FormattedPercentCoasting
        {
            get { return Formatting.FormattedPercentage(percentCoasting); }
        }
        public string FormattedFuelRemaining
        {
            get { return Formatting.FormattedPercentage(fuelRemaining); }
        }
        public string FormattedFuelUsed
        {
            get { return Formatting.FormattedPercentage(fuelUsed); }
        }
        public string FormattedDistanceTravelled
        {
            get { return $"{distanceTravelled.ToString("F2")}KM"; }
        }
        public string FormattedTotalDistanceTravelled
        {
            get { return $"{totalDistanceTravelled.ToString("F2")}KM"; }
        }
        public string FormattedAvgTyreWear
        {
            get { return $"{avgTyreWear}%"; }
        }
        public string FormattedAvgTyreRemaining
        {
            get { return $"{1 - avgTyreWear}%"; }
        }
        public double FlTyreRemaining
        {
            get { return Math.Round(100 - FlTyreWear,2); }
            set { value = FlTyreRemaining; }
        }
        public double FrTyreRemaining
        {
            get { return Math.Round(100 - FrTyreWear,2); }
            set { value = FrTyreRemaining; }
        }
        public double RlTyreRemaining
        {
            get { return Math.Round(100 - RlTyreWear,2); }
            set { value = RlTyreRemaining; }
        }
        public double RrTyreRemaining
        {
            get { return Math.Round(100 - RrTyreWear,2); }
            set { value = RrTyreRemaining; }
        }
        public string FormattedFuelAvailable { get { return Formatting.FormattedPercentage(1 - fuelUsed); } }
        public string FormattedTyreRemaining { get { return Formatting.FormattedPercentage((FlTyreRemaining + FrTyreRemaining + RlTyreRemaining + RrTyreRemaining) / 400); } }
        public string FormattedLapPositionChange
        {
            get
            {
                return (lapStartingPosition == racePosition) ? " = " : string.Format("{0}{1} ", lapStartingPosition > racePosition ? " +" : " -", (Math.Abs(lapStartingPosition - racePosition)));
            }
        }
        public string FormattedRacePosition
        {
            get { return $"{RacePosition} ({FormattedLapPositionChange})"; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
