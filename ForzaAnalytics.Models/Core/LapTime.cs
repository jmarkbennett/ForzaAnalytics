using System.ComponentModel;
using ForzaAnalytics.Models.Formatters;
namespace ForzaAnalytics.Models.Core
{
    public class LapTime : INotifyPropertyChanged
    {
        private float timeInSeconds;
        private int lapNumber;
        private bool isBestLap;
        private double averageSpeed;
        private double fuelRemaining;
        private double percentFullThrottle;
        private double percentBrakeApplied;
        private double minSpeed;
        private double maxSpeed;
        private double fuelUsed;
        private double percentCoasting;
        private int racePosition;
        public float TimeInSeconds
        {
            get { return timeInSeconds; }
            set
            {
                timeInSeconds = value;
                OnPropertyChanged(nameof(TimeInSeconds));
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

        public int RacePosition
        {
            get { return racePosition; }
            set
            {
                racePosition = value;
                OnPropertyChanged(nameof(RacePosition));
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

        public string FormattedLapTime
        {
            get { return Formatting.FormattedTime(timeInSeconds); }
        }

        public string FormattedAverageSpeed
        {
            get { return  $"{averageSpeed.ToString("F2")}MPH"; }
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
            get { return  Formatting.FormattedPercentage(percentFullThrottle); }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
