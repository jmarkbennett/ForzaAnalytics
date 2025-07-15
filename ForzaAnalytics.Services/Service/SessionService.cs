using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Service;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ForzaAnalytics.Services.Service
{
    public class SessionService
    {
        private LapDetailService lapSvc;
        private List<Car> cars;
        private List<Track> tracks;

        private ObservableCollection<Session> sessions;
        private Session currentSession;
        private Car sessionCar;
        private Track sessionTrack;

        private Telemetry lastTelemetry; // limitation of Data Out: we cant track the last lap when the race completes because it doesnts update the lap data for this. we have to get the last telemetry value which will be off

        public Session CurrentSession { get { return currentSession; } }

        public SessionService()
        {         
            Reset();
            SyncData();
            lapSvc.LapCompleteEvent += AddLapToCurrentSession;
        }

        public LapDetailService LapDetails { get { return lapSvc; } }

        public void Reset()
        {
            lapSvc = new LapDetailService();
            cars = CarDetailsSeralizer.LoadCarDetails();
            tracks = TrackDetailsSeralizer.LoadTrackDetails();
            currentSession = new Session();
            sessions = [];
        }
        private Car ObtainCarDetail(int carId)
        {
            var detail = new Models.Core.Car() { CarId = carId.ToString() };
            if (cars.Where(i => i.CarId == carId.ToString()).Count() > 0)
            {
                return cars.Where(i => i.CarId == carId.ToString()).FirstOrDefault();
            }
            else
            {
                return detail;
            }
        }
        private Track ObtainTrackDetail(int trackId)
        {
            var detail = new Track() { TrackId = trackId.ToString() };
            if (tracks.Where(i => i.TrackId == trackId.ToString()).Count() > 0)
            {
                return tracks.Where(i => i.TrackId == trackId.ToString()).FirstOrDefault();
            }
            else
            {
                return detail;
            }
        }
        private bool HasSessionChanged(ref Telemetry payload)
        {
            if (payload.DistanceTravelled < 0)
                return false;
            if (payload.Race.TrackIdentifier != currentSession.TrackId)
                return true;
            if (payload.Car.CarIdentifier != currentSession.CarId)
                return true;
            //if (payload.DistanceTravelled < currentSession.DistanceTravelled) // cant use this as driving in reverse or backwards reverses distance.
            //    return true;
            if (payload.Race.LapNumber < currentSession.CurrentLapNumber)
                return true;
            if (payload.Race.CurrentRaceTime < currentSession.LastSessionRaceTime)
                return true;

            return false;

        }
        public void Update(Telemetry payload)
        {
            lastTelemetry = payload;

            if (HasSessionChanged(ref payload))
            {
                if (currentSession.CarId > 0) // session exists, close it off, reset laps
                {
                    var endTime = DateTime.Now;
                    if (lapSvc.LapTimes.Any())
                        endTime = lapSvc.LapTimes.Last().TimeOfLapTime;
                    SessionSerializer.CloseSession(currentSession.SessionId, "Automatic", endTime);
                    currentSession.FinalizeSession();
                    sessions.Add(currentSession);
                    lapSvc.Reset();
                }

                currentSession = new Session();
                currentSession.InitSession(
                    payload.Car.CarIdentifier,
                    payload.Race.TrackIdentifier,
                    payload.Race.CurrentRaceTime,
                    int.Parse(payload.Car.CarPerformanceIndex),
                    Models.Formatters.Formatting.GetCarClass(payload.Car.CarClass).ToString()
                );
                sessionCar = ObtainCarDetail(payload.Car.CarIdentifier);
                sessionTrack = ObtainTrackDetail(payload.Race.TrackIdentifier);
            }

            if (currentSession.CarId > 0 && payload.DistanceTravelled > 0)
            {
                currentSession.DistanceTravelled = payload.DistanceTravelled;
                currentSession.LastSessionRaceTime = payload.Race.CurrentRaceTime;
                if (payload.Race.LapNumber != currentSession.CurrentLapNumber)
                    currentSession.CurrentLapNumber = payload.Race.LapNumber;

                lapSvc.Update(payload, currentSession.SessionId, SessionSummary);
            }
        }

        public void AddLapToCurrentSession(LapTime lap)
        {
            SessionSerializer.LogSessionRow(currentSession, lap);
        }

        public void CreateFinalRaceLap()
        {
            lapSvc.CreateFinalRaceLap(lastTelemetry, currentSession.SessionId, SessionSummary);
        }

        public void SyncData()
        {
           var readSessions = SessionSerializer.GetAllSessions();
            foreach (var sess in readSessions)
                sessions.Add(sess);
        }

        public ObservableCollection<LapTime> CurrentLapTimes { get { return lapSvc.LapTimes; } }

        public string SessionSummary { get { return $"{sessions.Count()} - Car: {sessionCar?.YearMakeModel ?? "Unknown"} - Track: {sessionTrack?.FullTrackName ?? "Unknown"}"; } }
    }
}