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

namespace ForzaAnalytics.Services.Service
{
    public class SessionService
    {
        private LapDetailService svc;
        private List<Car> cars;
        private List<Track> tracks;

        private float previousRaceTime;
        private ObservableCollection<Session> sessions;
        private Session session;

        private Car sessionCar;
        private Track sessionTrack;
        public void Update(Telemetry payload)
        {
            if (HasSessionChanged(ref payload))
            {
                if (session.CarId > 0) // session exists
                {
                    var endTime = DateTime.Now;
                    if (svc.LapTimes.Any())
                        endTime = svc.LapTimes.Last().TimeOfLapTime;
                    SessionSerializer.CloseSession(session.SessionId, "Automatic", endTime);
                    session.FinalizeSession();
                    sessions.Add(session);
                    svc.ResetService();
                }
                session = new Session();
                session.InitSession(payload.Car.CarIdentifier, payload.Race.TrackIdentifier, payload.Race.CurrentRaceTime, int.Parse(payload.Car.CarPerformanceIndex), 
                    Models.Formatters.Formatting.GetCarClass(payload.Car.CarClass).ToString());
                sessionCar = ObtainCarDetail(payload.Car.CarIdentifier);
                sessionTrack = ObtainTrackDetail(payload.Race.TrackIdentifier);
            }

            UpdateSession(ref payload);
            svc.Update(payload, session.SessionId, SessionSummary);
        }
        private bool HasSessionChanged(ref Telemetry payload)
        {
            if (payload.Race.TrackIdentifier != session.TrackId)
                return true;
            if (payload.Car.CarIdentifier != session.CarId)
                return true;
            //if (payload.DistanceTravelled < session.DistanceTravelled) // cant use this as driving in reverse or backwards reverses distance.
            //    return true;
            if (payload.Race.LapNumber < session.CurrentLapNumber)
                return true;
            if (payload.Race.CurrentRaceTime < session.LastSessionRaceTime)
                return true;

            return false;

        }
        private void UpdateSession(ref Telemetry payload)
        {
            session.DistanceTravelled = payload.DistanceTravelled;
            session.LastSessionRaceTime = payload.Race.CurrentRaceTime;
            if (payload.Race.LapNumber != session.CurrentLapNumber)
                session.CurrentLapNumber = payload.Race.LapNumber;
        }
        public void AddLapToCurrentSession(LapTime lap)
        {
            session.LapTimes.Add(lap);
            var bestTime = session.LapTimes.Min(i => i.TimeInSeconds);
            foreach (LapTime t in session.LapTimes)
                t.IsBestLap = (t.TimeInSeconds == bestTime);

            SessionSerializer.LogSessionRow(session, lap);
        }
        public void Reset()
        {
            cars = CarDetailsSeralizer.LoadCarDetails();
            tracks = TrackDetailsSeralizer.LoadTrackDetails();
            session = new Session();
            sessions = [];
            
            previousRaceTime = 0;
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

        public SessionService()
        {
            svc = new LapDetailService();
            session = new Session();
            Reset();
            SyncData();
            svc.LapCompleteEvent += AddLapToCurrentSession;
        }

        private void SyncData()
        {
           var readSessions = SessionSerializer.GetAllSessions();
            foreach (var sess in readSessions)
                sessions.Add(sess);
        }

        public ObservableCollection<LapTime> CurrentLapTimes { get { return svc.LapTimes; } }
        public Guid SessionId { get { return session.SessionId; } }
        public string SessionSummary { get { return $"{sessions.Count()} - Car: {sessionCar?.YearMakeModel ?? "Unknown"} - Track: {sessionTrack?.FullTrackName ?? "Unknown"}"; } }
    }
}