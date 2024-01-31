using ForzaAnalytics.Models.Core;
using ForzaAnalytics.UdpReader.Model;
using System.Collections.ObjectModel;

namespace ForzaAnalytics.Services.Service
{
    public class LapDetailService
    {
        public ObservableCollection<LapTime> LapTimes { get; set; }
        private List<double> currentSpeeds;
        private List<double> currentAccelerations;
        private List<double> currentBrakeApplied;
        private List<double> fuelConsumption;
        private int currentLapNumber;
        private float initialFuel = -1;
        public double AverageSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Average() : 0.0; } }
        public double PercentBrakeApplied { get { return ((double)currentBrakeApplied.Count(x => x > 0) / (double)currentBrakeApplied.Count()); } }
        public double PercentFullThrottle { get { return ((double)currentAccelerations.Count(x => x == 100) / (double)currentBrakeApplied.Count()); } }
        public double MaxSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Max() : 0.0; } }
        public double MinSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Min() : 0.0; } }
        public double FuelUsed { get { return fuelConsumption.Any() ? fuelConsumption.Max() - fuelConsumption.Min() : 0; } }
        private void ClearCurrentValues()
        {
            currentSpeeds = [];
            currentAccelerations = [];
            currentBrakeApplied = [];
            fuelConsumption = [];
        }
        private void AppendCurrentValues(ref Telemetry payload)
        {
            currentAccelerations.Add(payload.Acceleration);
            currentBrakeApplied.Add(payload.Brake);
            currentSpeeds.Add(payload.Speed_Mph);
            fuelConsumption.Add(payload.Fuel);
        }
        public LapDetailService()
        {
            LapTimes = [];
            currentSpeeds = [];
            currentAccelerations = [];
            currentBrakeApplied = [];
            fuelConsumption = [];
            currentLapNumber = 0;
        }
        public void Update(Telemetry payload)
        {
            if (initialFuel == -1)
                initialFuel = payload.Fuel;
            if (LapTimes.Any() && payload.Race.LastLapTime > 0)
            {
                if (LapTimes[LapTimes.Count - 1].TimeInSeconds != payload.Race.LastLapTime)
                {
                    LapTimes.Add(
                        new LapTime()
                        {
                            IsBestLap = (payload.Race.LastLapTime == payload.Race.BestLapTime),
                            LapNumber = payload.Race.LapNumber,
                            TimeInSeconds = payload.Race.LastLapTime,
                            FuelRemaining = payload.Fuel,
                            AverageSpeed = AverageSpeed,
                            PercentBrakeApplied = PercentBrakeApplied,
                            PercentFullThrottle = PercentFullThrottle,
                            MaxSpeed = MaxSpeed,
                            MinSpeed = MinSpeed,
                            FuelUsed = FuelUsed
                        }
                    );

                    foreach (LapTime t in LapTimes)
                        t.IsBestLap = (t.TimeInSeconds == payload.Race.BestLapTime);
                }
            }
            else if (payload.Race.LastLapTime > 0)
            {
                LapTimes.Add(
                    new LapTime()
                    {
                        IsBestLap = (payload.Race.LastLapTime == payload.Race.BestLapTime),
                        LapNumber = payload.Race.LapNumber,
                        TimeInSeconds = payload.Race.LastLapTime,
                        FuelRemaining = payload.Fuel,
                        AverageSpeed = AverageSpeed,
                        PercentBrakeApplied = PercentBrakeApplied,
                        PercentFullThrottle = PercentFullThrottle,
                        MaxSpeed = MaxSpeed,
                        MinSpeed = MinSpeed,
                        FuelUsed = FuelUsed
                    }
                );
            }

            if (currentLapNumber == payload.Race.LapNumber)
            {
                AppendCurrentValues(ref payload);
            }
            else
            {
                currentLapNumber = payload.Race.LapNumber;
                ClearCurrentValues();
                AppendCurrentValues(ref payload);
            }
        }
        public void ResetService()
        {
            LapTimes = new ObservableCollection<LapTime>();
            ClearCurrentValues();
            currentLapNumber = 0;
            initialFuel = -1;
        }
    }
}
