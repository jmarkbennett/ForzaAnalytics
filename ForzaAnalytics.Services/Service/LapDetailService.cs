using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace ForzaAnalytics.Services.Service
{
    public delegate void LapCompleteEventHandler(LapTime lap);
    public class LapDetailService
    {
        public event LapCompleteEventHandler LapCompleteEvent;
        public ObservableCollection<LapTime> LapTimes { get; set; }
        public List<LapTime> ReversedLapTimes { get { return LapTimes.Reverse().ToList(); } }
        private List<double> currentSpeeds;
        private List<double> currentAccelerations;
        private List<double> currentBrakeApplied;
        private List<int> currentPositions;
        private List<double> currentCoasting;
        private List<double> fuelConsumption;
        
        private double distanceTravelled;
        private int currentLapNumber;
        private float initialFuel = -1;
        private float initialDistance = -1;
        public double AverageSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Average() : 0.0; } }
        public double PercentBrakeApplied { get { return ((double)currentBrakeApplied.Count(x => x > 0) / (double)currentBrakeApplied.Count()); } }
        public double PercentFullThrottle { get { return ((double)currentAccelerations.Count(x => x == 100) / (double)currentAccelerations.Count()); } }
        public double MaxSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Max() : 0.0; } }
        public double MinSpeed { get { return currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Min() : 0.0; } }
        public double FuelUsed { get { return fuelConsumption.Any() ? fuelConsumption.Max() - fuelConsumption.Min() : 0; } }
        public double PercentCoasting { get { return ((double)currentCoasting.Count() / (double)currentAccelerations.Count()); } }
        public int PositionChanges { get { return currentPositions.Any() ? currentPositions.Distinct().Count() : 0; } }
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
            currentPositions.Add(payload.Race.RacePosition);
        }

        public LapDetailService()
        {
            LapTimes = new ObservableCollection<LapTime>();
            currentSpeeds = [];
            currentAccelerations = [];
            currentCoasting = [];
            currentBrakeApplied = [];
            fuelConsumption = [];
            currentPositions = [];
            currentLapNumber = 0;
            distanceTravelled = 0;

            SessionSerializer.initializeDatabase();

            SyncData(SessionSerializer.GetAllSessions(), SessionSerializer.GetAllLapTimes());

        }
        public void Update(Telemetry payload, Guid sessionId, string sessionSummary)
        {
            if (initialFuel == -1)
                initialFuel = payload.Fuel;
            if (initialDistance == -1)
                initialDistance = payload.DistanceTravelled;
            if (LapTimes.Any() && payload.Race.LastLapTime > 0)
            {
                if (LapTimes[LapTimes.Count - 1].TimeInSeconds != payload.Race.LastLapTime) // Scenario 1: Check the last lap time is different to the payloads last lap
                {
                    var current = new LapTime()
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
                        PercentCoasting = PercentCoasting,
                        TotalDistanceTravelled = payload.DistanceTravelled_Km,
                        DistanceTravelled = payload.DistanceTravelled_Km - LapTimes.Last().TotalDistanceTravelled,
                        AvgTyreWear = payload.Tire.AvgTireWear,
                        FlTyreWear = payload.Tire.TireWearFrontLeft,
                        FrTyreWear = payload.Tire.TireWearFrontRight,
                        RlTyreWear = payload.Tire.TireWearRearLeft,
                        RrTyreWear = payload.Tire.TireWearRearRight,
                        SessionSummary = sessionSummary,
                        PositionChanges = PositionChanges,
                        SessionId = sessionId,
                        TimeOfLapTime = DateTime.Now
                       
                    };
                    LapTimes.Add(current);
                    LapCompleteEvent?.Invoke(current);

                    foreach (LapTime t in LapTimes)
                        t.IsBestLap = (t.TimeInSeconds == payload.Race.BestLapTime);
                }
            }
            else if (payload.Race.LastLapTime > 0) // scenario 2: no last lap time
            {
                var current = new LapTime()
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
                    PercentCoasting = PercentCoasting,
                    TotalDistanceTravelled = payload.DistanceTravelled_Km,
                    DistanceTravelled = payload.DistanceTravelled_Km,
                    AvgTyreWear = payload.Tire.AvgTireWear,
                    FlTyreWear = payload.Tire.TireWearFrontLeft,
                    FrTyreWear = payload.Tire.TireWearFrontRight,
                    RlTyreWear = payload.Tire.TireWearRearLeft,
                    RrTyreWear = payload.Tire.TireWearRearRight,
                    SessionSummary = sessionSummary,
                    PositionChanges = PositionChanges,
                    SessionId = sessionId,
                    TimeOfLapTime = DateTime.Now
                };
                LapTimes.Add(current);
                LapCompleteEvent?.Invoke(current);
                
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
            distanceTravelled = 0;
        }

        public void SyncData(List<Session> sessions, List<LapTime> laps)
        {
            for(var i = 0; i< sessions.Count; i++) {
                var car = CarDetailsSeralizer.LoadCarDetails().Where(x=>x.CarId == sessions[i].CarId.ToString()).FirstOrDefault();
                var track = TrackDetailsSeralizer.LoadTrackDetails().Where(x=>x.TrackId == sessions[i].TrackId.ToString()).FirstOrDefault();
                var sessionLaps = laps.Where(x => x.SessionId == sessions[i].SessionId).ToList();
                foreach (var lap in sessionLaps)
                {
                    var toAdd = lap;
                    var sessionEnd = sessions[i].SessionEnd.ToString("yyyy-mm-dd HH:mm:ss");
                    if (sessions[i].SessionEnd.Year == 1)
                        sessionEnd = "???";
                    toAdd.SessionSummary = $"{i} - Car: {car?.YearMakeModel ?? "Unknown"} - Track: {track?.FullTrackName ?? "Unknown"} - ({sessions[i].SessionStart.ToString("yyyy-mm-dd HH:mm:ss")} - {sessionEnd})";
                    LapTimes.Add(toAdd);
                }
            }
        }
    }
}