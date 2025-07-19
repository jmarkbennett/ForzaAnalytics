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

        public int GetUniqueCars()
        {
            var cars = sessions.Select(x => x.CarId).Distinct().Count();
            return cars;
        }
        public int GetUniqueCircuits() // not layouts
        {
            var circuits = from s in sessions join t in tracks on s.TrackId.ToString() equals t.TrackId select t.Name;
            return circuits.Distinct().Count();
        }
        public double GetTotalDistance()
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

        public int GetSessions()
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


        public double MaximumSpeed()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Max(x => x.MaxSpeed) : 0;
        }
        public double AverageSpeed()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Average(x => x.AverageSpeed): 0;
        }

        public double AveragePosition()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Average(x => x.RacePosition) : 0;
        }

        public double AveragePositionChanges()
        {
            return lapSvc.LapTimes.Any() ? lapSvc.LapTimes.Average(x => x.PositionChanges): 0;
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

        public IList<Models.Stats.CarTrackDistance> DistanceByCar()
        {
            var result = new List<Models.Stats.CarTrackDistance>();

            var totalDistance = from lap in lapSvc.LapTimes join sess in sessions on lap.SessionId equals sess.SessionId
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
                          CarOrTrackName =  cars.Where(i=>i.CarId == grp.Key.ToString()).Select(x=>x.YearMakeModel).FirstOrDefault(),
                          DistanceTravelled = (from sub in grp select sub.Distance).Sum()
                      };

            return tmp.ToList();
        }
    }
}
/*

            < Label Grid.Row = "1" Grid.Column = "3" Height = "30" Content = "Average Lap Time By Track"     Name = "mAverageLapTimeByTrack" ></ Label >
            < Label Grid.Row = "1" Grid.Column = "4" Height = "30" Content = "Average Session Length"    Name = "lAverageSessionLength" ></ Label >



            < Label Grid.Row = "2" Grid.Column = "0"  Content = "Time Per Track" Name = "lTrackMostDriven" ></ Label >
            < Label Grid.Row = "2" Grid.Column = "3" Content = "Time Per Car" Name = "mTimePerCar" ></ Label >


            < Label Grid.Row = "3" Grid.Column = "3" Content = "Average Fuel Consumption By Car"  Name = "mAverageFuelUseByCar" ></ Label > 
 */