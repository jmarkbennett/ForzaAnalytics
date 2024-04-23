using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace ForzaAnalytics.Services.Service
{
    public class LapDetailService
    {
        public ObservableCollection<LapTime> LapTimes { get; set; }
        private List<double> currentSpeeds;
        private List<double> currentAccelerations;
        private List<double> currentBrakeApplied;
        private List<double> currentCoasting;
        private List<double> fuelConsumption;
        private int currentLapNumber;
        private float initialFuel = -1;
        public double AverageSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Average() : 0.0; } }
        public double PercentBrakeApplied { get { return ((double)currentBrakeApplied.Count(x => x > 0) / (double)currentBrakeApplied.Count()); } }
        public double PercentFullThrottle { get { return ((double)currentAccelerations.Count(x => x == 100) / (double)currentAccelerations.Count()); } }
        public double MaxSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Max() : 0.0; } }
        public double MinSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Min() : 0.0; } }
        public double FuelUsed { get { return fuelConsumption.Any() ? fuelConsumption.Max() - fuelConsumption.Min() : 0; } }
        public double PercentCoasting { get { return ((double)currentCoasting.Count() / (double)currentAccelerations.Count()); } }
        private void ClearCurrentValues()
        {
            currentSpeeds = [];
            currentAccelerations = [];
            currentCoasting = [];
            currentBrakeApplied = [];
            fuelConsumption = [];
        }
        private void AppendCurrentValues(ref Telemetry payload)
        {
            currentAccelerations.Add(payload.Acceleration);
            if (payload.Acceleration == 0 && payload.Brake == 0 && payload.Handbrake == 0)
                currentCoasting.Add(0);
            currentBrakeApplied.Add(payload.Brake);
            currentSpeeds.Add(payload.Speed_Mph);
            fuelConsumption.Add(payload.Fuel);
        }

        public LapDetailService()
        {
            LapTimes = [];
            currentSpeeds = [];
            currentAccelerations = [];
            currentCoasting = [];
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
                            RacePosition = payload.Race.RacePosition,
                            TimeInSeconds = payload.Race.LastLapTime,
                            FuelRemaining = payload.Fuel,
                            AverageSpeed = AverageSpeed,
                            PercentBrakeApplied = PercentBrakeApplied,
                            PercentFullThrottle = PercentFullThrottle,
                            MaxSpeed = MaxSpeed,
                            MinSpeed = MinSpeed,
                            FuelUsed = FuelUsed,
                            PercentCoasting = PercentCoasting
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
                        RacePosition = payload.Race.RacePosition,
                        TimeInSeconds = payload.Race.LastLapTime,
                        FuelRemaining = payload.Fuel,
                        AverageSpeed = AverageSpeed,
                        PercentBrakeApplied = PercentBrakeApplied,
                        PercentFullThrottle = PercentFullThrottle,
                        MaxSpeed = MaxSpeed,
                        MinSpeed = MinSpeed,
                        FuelUsed = FuelUsed,
                        PercentCoasting = PercentCoasting
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

        public void ImportTelemetry(string filename)
        {
            ResetService();
            var telemetry = MapSerializer.LoadPositionData(filename);
            var laps = telemetry.ExtendedPositions.Select(i => i.LapNumber).Distinct().ToList();
            foreach(var lap in laps)
            {
                var row = new LapTime();
                var lapInfo = telemetry.ExtendedPositions.Select(i => i).Where(i => i.LapNumber == lap).ToList();
                row.LapNumber = lap;
                row.AverageSpeed = lapInfo.Average(i => i.Speed_Mph);
                row.MaxSpeed = lapInfo.Max(i => i.Speed_Mph);
                row.MinSpeed = lapInfo.Min(i => i.Speed_Mph);
                row.TimeInSeconds = lapInfo.Max(i => i.LapTime);
                row.PercentBrakeApplied = (double)lapInfo.Where(i => i.Brake > 0).Count() / (double)lapInfo.Count();
                row.PercentFullThrottle = (double)lapInfo.Where(i => i.Acceleration == 100).Count() / (double)lapInfo.Count();
                row.PercentCoasting = (double)lapInfo.Where(i => i.Acceleration == 0 && i.Brake == 0 && i.Handbrake == 0).Count() / (double)lapInfo.Count();
                row.FuelRemaining = (double)lapInfo.Min(i => i.FuelRemaining);
                row.FuelUsed = (double)lapInfo.Max(i => i.FuelRemaining) - (double)lapInfo.Min(i => i.FuelRemaining);
                row.RacePosition = lapInfo.Last().RacePosition;
                LapTimes.Add(row);
            }

            if (laps.Count > 0)
            {
                var bestLap = LapTimes.Min(i => i.TimeInSeconds);
                foreach (var lap in LapTimes)
                {
                    if (lap.TimeInSeconds == bestLap)
                        lap.IsBestLap = true;
                }
            }
        }
    }
}