using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Service;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ForzaAnalytics.Services.Service
{
    public class StatsService
    {
        private LapDetailService lapSvc;
        private List<Car> cars;
        private List<Track> tracks;

        private IList<Session> sessions;

        public StatsService()
        {
            Reset();
            SyncData();
        }


        public void Reset()
        {
            lapSvc = new LapDetailService();
            cars = CarDetailsSeralizer.LoadCarDetails();
            tracks = TrackDetailsSeralizer.LoadTrackDetails();
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
        public void SyncData()
        {
            var readSessions = SessionSerializer.GetAllSessions();
            foreach (var sess in readSessions)
                sessions.Add(sess);

            lapSvc.SyncData();
        }
        public int UniqueCars()
        {
            var cars = sessions.Select(x => x.CarId).Distinct().Count();
            return cars;
        }
        public int UniqueCircuits() // not layouts
        {
            var circuits = from s in sessions join t in tracks on s.TrackId.ToString() equals t.TrackId select t.Name;
            return circuits.Distinct().Count();
        }
        public double TotalDistance()
        {
            var totalDistance = from lap in lapSvc.LapTimes
                                group lap by lap.SessionId into grp
                                select new
                                {
                                    Sessionid = grp.Key,
                                    Distance = (from sub in grp select sub.TotalDistanceTravelled).Max()

                                };

            return Math.Round(totalDistance.Select(x => x.Distance).Sum(), 2);
        }
        public int UniqueSessions()
        {
            return sessions.Count();
        }
        public int TotalLaps()
        {
            var totalLaps = from lap in lapSvc.LapTimes
                            group lap by lap.SessionId into grp
                            select new
                            {
                                Sessionid = grp.Key,
                                Laps = (from sub in grp select sub.LapNumber).Max()

                            };

            return totalLaps.Select(x => x.Laps).Sum();
        }
        public int TotalTyreChanges()
        {
            var instances = 0;
            foreach (var session in sessions)
            {
                var laps = lapSvc.LapTimes.Where(x => x.SessionId == session.SessionId).ToList();
                var prevAvgWear = 100.00;
                foreach (var lap in laps)
                {
                    if (prevAvgWear == 100)
                        prevAvgWear = lap.AvgTyreWear;
                    else
                    {
                        if (prevAvgWear < lap.AvgTyreWear)
                            instances++;
                    }
                }
            }
            return instances;
        }
        public int TotalFuelStops()
        {
            var instances = 0;
            foreach (var session in sessions)
            {
                var laps = lapSvc.LapTimes.Where(x => x.SessionId == session.SessionId).ToList();
                var prevFuel = 100.00;
                foreach (var lap in laps)
                {
                    if (prevFuel == 100)
                        prevFuel = lap.FuelUsed;
                    else
                    {
                        if (prevFuel < lap.FuelUsed)
                            instances++;
                    }
                }
            }
            return instances;
        }
        public double MaximumSpeed()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Max(x => x.MaxSpeed) : 0;
        }
        public double AverageSpeed()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Average(x => x.AverageSpeed) : 0;
        }
        public double AveragePosition()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Average(x => x.RacePosition) : 0;
        }
        public double AveragePositionChanges()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Average(x => x.PositionChanges) : 0;
        }
        public int Wins()
        {
            var wins = 0;
            foreach (var session in sessions)
            {
                if (session.SessionEndType == "ManualRaceComplete")
                {
                    if (lapSvc.LapTimes.Where(x => x.SessionId == session.SessionId).Last().RacePosition == 1)
                        wins++;
                }
            }

            return wins;
        }
        public int SecondPlaces()
        {
            var seconds = 0;
            foreach (var session in sessions)
            {
                if (session.SessionEndType == "ManualRaceComplete")
                {
                    if (lapSvc.LapTimes.Where(x => x.SessionId == session.SessionId).Last().RacePosition == 2)
                        seconds++;
                }
            }

            return seconds;
        }
        public int ThirdPlaces()
        {
            var thirds = 0;
            foreach (var session in sessions)
            {
                if (session.SessionEndType == "ManualRaceComplete")
                {
                    if (lapSvc.LapTimes.Where(x => x.SessionId == session.SessionId).Last().RacePosition == 3)
                        thirds++;
                }
            }

            return thirds;
        }
        public int Podiums()
        {
            var podiums = 0;
            foreach (var session in sessions)
            {
                if (session.SessionEndType == "ManualRaceComplete")
                {
                    if (lapSvc.LapTimes.Where(x => x.SessionId == session.SessionId).Last().RacePosition <= 3)
                        podiums++;
                }
            }

            return podiums;
        }
        public string TotalTimeTracked()
        {
            TimeSpan timeInSeconds = new TimeSpan();
            foreach (var session in sessions)
            {
                if (!string.IsNullOrEmpty(session.SessionEndType))
                {
                    if (timeInSeconds.TotalSeconds == 0)
                        timeInSeconds = session.SessionEnd - session.SessionStart;
                    else
                        timeInSeconds += session.SessionEnd - session.SessionStart;
                }
            }
            return timeInSeconds.ToString(@"hh\:mm\:ss");
        }
        public int LapsLead()
        {
            return lapSvc.LapTimes.Where(x => x.RacePosition == 1).Count();
        }

        public string AverageSessionTime()
        {
            TimeSpan timeInSeconds = new TimeSpan();
            foreach (var session in sessions)
            {
                if (!string.IsNullOrEmpty(session.SessionEndType))
                {
                    if (timeInSeconds.TotalSeconds == 0)
                        timeInSeconds = session.SessionEnd - session.SessionStart;
                    else
                        timeInSeconds += session.SessionEnd - session.SessionStart;
                }
            }

            return timeInSeconds.Divide(sessions.Any() ? 
                sessions
                .Where(x => !string.IsNullOrEmpty(x.SessionEndType))
                .Count() : 1)
                .ToString(@"hh\:mm\:ss");
        }

        public double AverageSessionDistance()
        {
            //  double totalDistance = 0.0;
            //   foreach (var session in sessions)
            //  {
            //      if (!string.IsNullOrEmpty(session.SessionEndType))
            //      {
            //          if (totalDistance == 0.0)
            //              totalDistance = session.SessionEnd - session.SessionStart;
            //          else
            //              timeInSeconds += session.SessionEnd - session.SessionStart;
            //     }
            // }
            // return timeInSeconds.Divide(sessions.Where(x => !string.IsNullOrEmpty(x.SessionEndType)).Count()).ToString(@"hh\:mm\:ss");
            return 0.0;
        }
        public IList<Models.Stats.CarTrackDistance> TotalDistanceByCar()
        {
            var result = new List<Models.Stats.CarTrackDistance>();

            var totalDistance = from lap in lapSvc.LapTimes
                                join sess in sessions on lap.SessionId equals sess.SessionId
                                group lap by new { lap.SessionId, sess.CarId } into grp
                                select new
                                {
                                    Sessionid = grp.Key.SessionId,
                                    CarId = grp.Key.CarId,
                                    Distance = (from sub in grp select sub.TotalDistanceTravelled).Max()

                                };

            var tmp = from td in totalDistance
                      group td by td.CarId into grp
                      select new Models.Stats.CarTrackDistance
                      {
                          CarOrTrackName = cars.Where(i => i.CarId == grp.Key.ToString()).Select(x => x.YearMakeModel).FirstOrDefault(),
                          DistanceTravelled = (from sub in grp select sub.Distance).Sum()
                      };

            return tmp.ToList();
        }
        public IList<Models.Stats.CarTrackDuration> TotalTimeTrackedByCar()
        {
            var result = new List<Models.Stats.CarTrackDuration>();
            var uniqueCars = (from session in sessions where !string.IsNullOrEmpty(session.SessionEndType) select session.CarId).Distinct();

            foreach(var car in uniqueCars)
            {
                var row = new Models.Stats.CarTrackDuration();
                row.CarOrTrackName = cars.Where(x => x.CarId == car.ToString()).FirstOrDefault().YearMakeModel;
                row.Duration = new TimeSpan();
                foreach (var session in sessions.Where(x=>x.CarId == car))
                {
                   if(row.Duration.TotalSeconds == 0)
                        row.Duration = session.SessionEnd - session.SessionStart;
                   else
                        row.Duration += session.SessionEnd - session.SessionStart;
                }

                result.Add(row);
            }
            return result;
        }
        public IList<Models.Stats.CarTrackGenericStat> AveragePositionByCar()
        {
            var result = new List<Models.Stats.CarTrackGenericStat>();
            var uniqueCars = (from session in sessions where !string.IsNullOrEmpty(session.SessionEndType) select session.CarId).Distinct();

            foreach (var car in uniqueCars)
            {
                var row = new Models.Stats.CarTrackGenericStat();
                row.CarOrTrackName = cars.Where(x => x.CarId == car.ToString()).FirstOrDefault().YearMakeModel;
                row.StatValue = "0";
                var laps = from lap in lapSvc.LapTimes join sess in sessions on lap.SessionId equals sess.SessionId where sess.CarId == car select lap;

                foreach (var lap in laps)
                {
                    if (row.StatValue == "0")
                        row.StatValue = lap.RacePosition.ToString();
                    else
                        row.StatValue = $"{int.Parse(row.StatValue) + lap.RacePosition}";
                }
                row.StatValue = $"{Math.Round(double.Parse(row.StatValue) / laps.Count(), 2)}";
                result.Add(row);
            }
            return result;
        }
        public IList<Models.Stats.CarTrackDuration> TotalTimeTrackedByCarDivision()
        {
            var result = new List<Models.Stats.CarTrackDuration>();
            var uniqueClasses = (from session in sessions join car in cars on session.CarId.ToString() equals car.CarId select car.Division).Distinct();
            
            foreach (var uniqueClass in uniqueClasses)
            {
                var row = new Models.Stats.CarTrackDuration();
                row.CarOrTrackName = uniqueClass;

                row.Duration = new TimeSpan();
                var sessionList = from session in sessions join car in cars on session.CarId.ToString() equals car.CarId where car.Division == uniqueClass select new {
                    CarDivision = car.Division,
                    Session = session
                };

                foreach (var session in sessionList)
                {
                    if (row.Duration.TotalSeconds == 0)
                        row.Duration = session.Session.SessionEnd - session.Session.SessionStart;
                    else
                        row.Duration += session.Session.SessionEnd - session.Session.SessionStart;
                }

                result.Add(row);
            }
            return result;
        }
        public IList<Models.Stats.CarTrackDistance> TotalDistinceByDivision()
        {
            var result = new List<Models.Stats.CarTrackDistance>();

            var totalDistance = from lap in lapSvc.LapTimes
                                join sess in sessions on lap.SessionId equals sess.SessionId
                                join car in cars on sess.CarId.ToString() equals car.CarId
                                group lap by new { sess.SessionId, car.Division } into grp
                                select new
                                {
                                    SessionId = grp.Key.SessionId,
                                    CarDivision = grp.Key.Division,
                                    Distance = (from sub in grp select sub.TotalDistanceTravelled).Max()
                                };

            var tmp = from td in totalDistance
                      group td by td.CarDivision into grp
                      select new Models.Stats.CarTrackDistance
                      {
                          CarOrTrackName = grp.Key,
                          DistanceTravelled = (from sub in grp select sub.Distance).Sum()
                      };

            return tmp.ToList();
        }
        public IList<Models.Stats.CarTrackGenericStat> AverageFuelConsumptionByCar()
        {
            var result = new List<Models.Stats.CarTrackGenericStat>();
            var uniqueCars = (from session in sessions where !string.IsNullOrEmpty(session.SessionEndType) select session.CarId).Distinct();

            foreach (var car in uniqueCars)
            {
                var row = new Models.Stats.CarTrackGenericStat();
                row.CarOrTrackName = cars.Where(x => x.CarId == car.ToString()).FirstOrDefault().YearMakeModel;
                row.StatValue = "0";
                var laps = from lap in lapSvc.LapTimes join sess in sessions on lap.SessionId equals sess.SessionId where sess.CarId == car select lap;

                foreach (var lap in laps)
                {
                    if (row.StatValue == "0")
                        row.StatValue = lap.FuelUsed.ToString();
                    else
                        row.StatValue = $"{double.Parse(row.StatValue) + lap.FuelUsed}";
                }
                row.StatValue = $"{Math.Round(double.Parse(row.StatValue) / laps.Count(), 2)}";
                result.Add(row);
            }
            return result;
        }

        public IList<Models.Stats.CarTrackDuration> TotalTimeTrackedByCircuit()
        {
            var result = new List<Models.Stats.CarTrackDuration>();
            var uniqueCircuits = (from session in sessions 
                                  join track in tracks on session.TrackId.ToString() equals track.TrackId 
                                  where !string.IsNullOrEmpty(session.SessionEndType) select track.Name).Distinct();

            foreach (var circuit in uniqueCircuits)
            {
                var row = new Models.Stats.CarTrackDuration();
                row.CarOrTrackName = circuit;
                row.Duration = new TimeSpan();
                var circuitSessions = from session in sessions
                                      join t in tracks on session.TrackId.ToString() equals t.TrackId
                                      where t.Name == circuit
                                       select session;

                foreach(var session in circuitSessions)
                {
                    if (row.Duration.TotalSeconds == 0)
                        row.Duration = session.SessionEnd - session.SessionStart;
                    else
                        row.Duration += session.SessionEnd - session.SessionStart;
                }

                result.Add(row);
            }
            return result;
        }
        public IList<Models.Stats.CarTrackDistance> TotalDistinceByCircuit()
        {
            var result = new List<Models.Stats.CarTrackDistance>();

            var totalDistance = from lap in lapSvc.LapTimes
                                join sess in sessions on lap.SessionId equals sess.SessionId
                                join track in tracks on sess.TrackId.ToString() equals track.TrackId
                                group lap by new { sess.SessionId, track.Name } into grp
                                select new
                                {
                                    SessionId = grp.Key.SessionId,
                                    CarDivision = grp.Key.Name,
                                    Distance = (from sub in grp select sub.TotalDistanceTravelled).Max()
                                };

            var tmp = from td in totalDistance
                      group td by td.CarDivision into grp
                      select new Models.Stats.CarTrackDistance
                      {
                          CarOrTrackName = grp.Key,
                          DistanceTravelled = (from sub in grp select sub.Distance).Sum()
                      };

            return tmp.ToList();
        }
        public IList<Models.Stats.CarTrackGenericStat> AveragePositionByCircuit()
        {
            var result = new List<Models.Stats.CarTrackGenericStat>();
            var uniqueCircuits = (from session in sessions join track in tracks on session.TrackId.ToString() equals track.TrackId select track.Name).Distinct();
            foreach(var circuit in uniqueCircuits)
            {
                var row = new Models.Stats.CarTrackGenericStat();
                row.CarOrTrackName = circuit;
                row.StatValue = "0";
                var laps = from lap in lapSvc.LapTimes
                           join sess in sessions on lap.SessionId equals sess.SessionId
                           join track in tracks on sess.TrackId.ToString() equals track.TrackId
                           where track.Name == circuit
                           select lap;
                foreach(var lap in laps)
                {
                    if (row.StatValue == "0")
                        row.StatValue = lap.RacePosition.ToString();
                    else
                        row.StatValue = $"{int.Parse(row.StatValue) + lap.RacePosition}";
                }
                row.StatValue = $"{Math.Round(double.Parse(row.StatValue) / laps.Count(), 2)}";
                result.Add(row);

            }
            return result;
        }

        public IList<Models.Stats.CarTrackDuration> TotalTimeTrackedByTrack()
        {
            var result = new List<Models.Stats.CarTrackDuration>();
            var uniqueTracks = (from session in sessions where !string.IsNullOrEmpty(session.SessionEndType) select session.TrackId).Distinct();

            foreach (var track in uniqueTracks)
            {
                var row = new Models.Stats.CarTrackDuration();
                row.CarOrTrackName = tracks.Where(x => x.TrackId == track.ToString()).FirstOrDefault().FullTrackName;
                row.Duration = new TimeSpan();
                foreach (var session in sessions.Where(x => x.TrackId == track))
                {
                    if (row.Duration.TotalSeconds == 0)
                        row.Duration = session.SessionEnd - session.SessionStart;
                    else
                        row.Duration += session.SessionEnd - session.SessionStart;
                }

                result.Add(row);
            }
            return result;
        }
        public IList<Models.Stats.CarTrackDistance> TotalDistinceByTrack()
        {
            var result = new List<Models.Stats.CarTrackDistance>();

            var totalDistance = from lap in lapSvc.LapTimes
                                join sess in sessions on lap.SessionId equals sess.SessionId
                                join track in tracks on sess.TrackId.ToString() equals track.TrackId
                                group lap by new { sess.SessionId, track.FullTrackName } into grp
                                select new
                                {
                                    SessionId = grp.Key.SessionId,
                                    CarDivision = grp.Key.FullTrackName,
                                    Distance = (from sub in grp select sub.TotalDistanceTravelled).Max()
                                };

            var tmp = from td in totalDistance
                      group td by td.CarDivision into grp
                      select new Models.Stats.CarTrackDistance
                      {
                          CarOrTrackName = grp.Key,
                          DistanceTravelled = (from sub in grp select sub.Distance).Sum()
                      };

            return tmp.ToList();
        }
        public IList<Models.Stats.CarTrackGenericStat> AveragePositionByTrack()
        {
            var result = new List<Models.Stats.CarTrackGenericStat>();
            var uniqueCircuits = (from session in sessions join track in tracks on session.TrackId.ToString() equals track.TrackId select track.FullTrackName).Distinct();
            foreach (var circuit in uniqueCircuits)
            {
                var row = new Models.Stats.CarTrackGenericStat();
                row.CarOrTrackName = circuit;
                row.StatValue = "0";
                var laps = from lap in lapSvc.LapTimes
                           join sess in sessions on lap.SessionId equals sess.SessionId
                           join track in tracks on sess.TrackId.ToString() equals track.TrackId
                           where track.FullTrackName == circuit
                           select lap;
                foreach (var lap in laps)
                {
                    if (row.StatValue == "0")
                        row.StatValue = lap.RacePosition.ToString();
                    else
                        row.StatValue = $"{int.Parse(row.StatValue) + lap.RacePosition}";
                }
                row.StatValue = $"{Math.Round(double.Parse(row.StatValue) / laps.Count(), 2)}";
                result.Add(row);

            }
            return result;
        }
    }
}