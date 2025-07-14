using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Enumerators;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace ForzaAnalytics.Services.Serializers
{
    public static class SessionSerializer
    {
        private static string name = "MyStats.sqlite";
        private static string connectionString = $"Data Source={name};Version=3;";

        private static void clearSessions()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM SessionData";
                using (var command = new SQLiteCommand(sql, conn))
                    command.ExecuteNonQuery();

                sql = "DELETE FROM LapData";
                using (var command = new SQLiteCommand(sql, conn))
                    command.ExecuteNonQuery();

                conn.Close();
            }
        }
        public static void initializeDatabase()
        {
            if (!File.Exists(name))
            {
                SQLiteConnection.CreateFile(name);
            }
            //resetDbTables();
            clearSessions();
            createDbTables();
        }

        private static void resetDbTables()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var sql = "DROP TABLE IF EXISTS SessionData";
                var command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();

                sql = "DROP TABLE IF EXISTS LapData";
                command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
        }
        private static void createDbTables()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var sql = @"CREATE TABLE IF NOT EXISTS SessionData(
                        SessionId TEXT NOT NULL,
                        SessionStartTime NUMERIC,
                        SessionEndTime NUMERIC,
                        SessionEndType TEXT,
                        TrackId INT NOT NULL,
                        CarId INT NOT NULL
                    )
                ";
                var command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();

                sql = @"
                        CREATE TABLE IF NOT EXISTS LapData (
                            SessionId TEXT NOT NULL,
                            CarClass TEXT,
                            CarPi INT,
                            TimeInSeconds REAL,
                            TimeOfLapTime REAL,
                            LapNumber INT,
                            LapPosition INT,
                            DistanceTravelled REAL,
                            TotalDistanceTravelled REAL,
                            AverageSpeed REAL,
                            MaxSpeed REAL,
                            MinSpeed REAL,
                            PercentFulLThrottle REAL,
                            PercentBrakeApplied REAL,
                            PercentCoasting REAL,
                            FuelRemaining REAL,
                            FuelUsed REAL,
                            flTyreWear REAL,
                            frTyreWear REAL,
                            rlTyreWear REAL,
                            rrTyreWear REAL,
                            positionChanges INT             
                    )";
                command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void LogSessionRow(Session session, LapTime lap)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO SessionData (SessionId, SessionStartTime, SessionEndTime, TrackId, CarId) " +
                                "SELECT @SessionId, @SessionStartTime, @SessionEndTime, @TrackId, @CarId " +
                                "WHERE NOT EXISTS (SELECT 1 FROM SessionData WHERE SessionId = @SessionId)";
                using (var command = new SQLiteCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@SessionId", session.SessionId.ToString());
                    command.Parameters.AddWithValue("@SessionStartTime", session.SessionStart.Ticks);
                    command.Parameters.AddWithValue("@SessionEndTime", session.SessionEnd.Ticks);
                    command.Parameters.AddWithValue("@TrackId", session.TrackId);
                    command.Parameters.AddWithValue("@CarId", session.CarId);
                    command.ExecuteNonQuery();
                }


                sql = "INSERT INTO LapData (SessionId, CarClass, CarPi, TimeOfLapTime, TimeInSeconds, LapNumber, LapPosition, " +
                    "DistanceTravelled, TotalDistanceTravelled, AverageSpeed, MaxSpeed, MinSpeed, " +
                    "PercentFullThrottle, PercentBrakeApplied, PercentCoasting, FuelRemaining, FuelUsed, " +
                    "flTyreWear, frTyreWear, rlTyreWear, rrTyreWear, positionChanges)" +
                    " SELECT @SessionId, @CarClass, @CarPi, @TimeOfLapTime, @TimeInSeconds, " +
                    "@LapNumber, @LapPosition, @DistanceTravelled, @TotalDistanceTravelled, @AverageSpeed, @MaxSpeed, @MinSpeed, " +
                    "@PercentFullThrottle, @PercentBrakeApplied, @PercentCoasting, @FuelRemaining, @FuelUsed, " +
                    "@flTyreWear, @frTyreWear, @rlTyreWear, @rrTyreWear, @positionChanges;";

                using (var command = new SQLiteCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@SessionId", session.SessionId.ToString());
                    command.Parameters.AddWithValue("@CarClass", session.CarClass);
                    command.Parameters.AddWithValue("@CarPi", session.CarPi);
                    command.Parameters.AddWithValue("@TimeOfLapTime", lap.TimeOfLapTime.Ticks);
                    command.Parameters.AddWithValue("@TimeInSeconds", lap.TimeInSeconds);
                    command.Parameters.AddWithValue("@LapNumber", lap.LapNumber);
                    command.Parameters.AddWithValue("@LapPosition", lap.RacePosition);
                    command.Parameters.AddWithValue("@DistanceTravelled", lap.DistanceTravelled);
                    command.Parameters.AddWithValue("@TotalDistanceTravelled", lap.TotalDistanceTravelled);
                    command.Parameters.AddWithValue("@AverageSpeed", lap.AverageSpeed);
                    command.Parameters.AddWithValue("@MaxSpeed", lap.MaxSpeed);
                    command.Parameters.AddWithValue("@MinSpeed", lap.MinSpeed);
                    command.Parameters.AddWithValue("@PercentFullThrottle", lap.PercentFullThrottle);
                    command.Parameters.AddWithValue("@PercentBrakeApplied", lap.PercentBrakeApplied);
                    command.Parameters.AddWithValue("@PercentCoasting", lap.PercentCoasting);
                    command.Parameters.AddWithValue("@FuelRemaining", lap.FuelRemaining);
                    command.Parameters.AddWithValue("@FuelUsed", lap.FuelUsed);
                    command.Parameters.AddWithValue("@flTyreWear", lap.FlTyreWear);
                    command.Parameters.AddWithValue("@frTyreWear", lap.FrTyreWear);
                    command.Parameters.AddWithValue("@rlTyreWear", lap.RlTyreWear);
                    command.Parameters.AddWithValue("@rrTyreWear", lap.RrTyreWear);
                    command.Parameters.AddWithValue("@positionChanges", lap.PositionChanges);

                    if (lap.MaxSpeed > 0)
                    {
                        command.ExecuteNonQuery();
                    }
                }

                conn.Close();
            }
        }

        public static void CloseSession(Guid sessionId, string endReason, DateTime endTime)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE SessionData SET SessionEndTime = @SessionEndTime, SessionEndType = @EndReason WHERE SessionId = @SessionId";
                using (var command = new SQLiteCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@SessionId", sessionId.ToString());
                    command.Parameters.AddWithValue("@SessionEndTime", endTime.Ticks);
                    command.Parameters.AddWithValue("@EndReason", endReason);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public static List<Session> GetAllSessions()
        {
            var sessions = new List<Session>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT DISTINCT s.SessionId SessionId, TrackId, CarId, SessionStartTime, SessionEndTime, SessionEndType, CarClass, CarPi  FROM SessionData s INNER JOIN LapData ld ON ld.Sessionid = ld.SessionId";
                using (var command = new SQLiteCommand(sql, conn))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new Session();
                        row.SessionId = new Guid(Convert.ToString(reader["SessionId"]));
                        row.TrackId = Convert.ToInt32(reader["TrackId"]);
                        row.CarId = Convert.ToInt32(reader["CarId"]);
                        row.CarClass = Convert.ToString(reader["CarClass"]);
                        row.CarPi = Convert.ToInt32(reader["CarPi"]);
                        row.SessionStart = new DateTime(Convert.ToInt64(reader["SessionStartTime"]));
                        row.SessionEnd = new DateTime(Convert.ToInt64(reader["SessionEndTime"]));
                        row.SessionEndType = Convert.ToString(reader["SessionEndType"]);
                        sessions.Add(row);
                    }
                }
                conn.Close();
            }
            return sessions;
        }

        public static List<LapTime> GetAllLapTimes()
        {
            var sessions = new List<LapTime>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT DISTINCT * FROM LapData WHERE MaxSpeed > 0";
                using (var command = new SQLiteCommand(sql, conn))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new LapTime();
                        row.SessionId = new Guid(Convert.ToString(reader["SessionId"]));
                        row.TimeInSeconds = Convert.ToSingle(reader["TimeInSeconds"]);
                        row.TimeOfLapTime = new DateTime(Convert.ToInt64(reader["TimeOfLapTime"]));
                        row.LapNumber = Convert.ToInt32(reader["LapNumber"]);
                        row.RacePosition = Convert.ToInt32(reader["LapPosition"]);
                        row.DistanceTravelled = Convert.ToDouble(reader["DistanceTravelled"]);
                        row.TotalDistanceTravelled = Convert.ToDouble(reader["TotalDistanceTravelled"]);
                       
                        row.AverageSpeed = Convert.ToDouble(reader["AverageSpeed"]);
                        row.MaxSpeed = Convert.ToDouble(reader["MaxSpeed"]);
                        row.MinSpeed = Convert.ToDouble(reader["MinSpeed"]);
                        row.PercentFullThrottle = Convert.ToDouble(reader["PercentFullThrottle"]);
                        row.PercentBrakeApplied = Convert.ToDouble(reader["PercentBrakeApplied"]);
                        row.PercentCoasting = Convert.ToDouble(reader["PercentCoasting"]);
                        row.FuelRemaining = Convert.ToDouble(reader["FuelRemaining"]);
                        row.FuelUsed = Convert.ToDouble(reader["FuelUsed"]);
                        row.FlTyreWear = Convert.ToDouble(reader["FlTyreWear"]);
                        row.FrTyreWear = Convert.ToDouble(reader["FrTyreWear"]);
                        row.RlTyreWear = Convert.ToDouble(reader["RlTyreWear"]);
                        row.RrTyreWear = Convert.ToDouble(reader["RrTyreWear"]);
                        row.PositionChanges = Convert.ToInt32(reader["PositionChanges"]);
                        sessions.Add(row);
                    }
                }
                conn.Close();
            }
            return sessions;
        }

        public static void ExportSessionData(string filePath) // need to move this data, or centralise it as the lap detail service also uses it
        {
            var sessions = GetAllSessions();
            var laps = GetAllLapTimes();


            using(var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(
                    "SessionId, CarId, TrackId, CarClass, CarPi, SessionStart, SessionEnd, SessionEndType, CurrentLapNumber," +
                    "TimeInSeconds, TimeOfLapTime, LapNumber, IsBestLap, DistanceTravelled, TotalDistanceTravelled," +
                    "AverageSpeed, FuelRemaining, AvgTyreWear, FlTyreWear, FrTyreWear, RlTyreWear, RrTyreWear, " +
                    "PercentFullThrottle, PercentBrakeApplied, MinSpeed, MaxSpeed, FuelUsed,PercentCoasting, PositionChanges, RacePosition, SessionSummary");
                for (var i = 0; i < sessions.Count; i++)
                {
                    var car = CarDetailsSeralizer.LoadCarDetails().Where(x => x.CarId == sessions[i].CarId.ToString()).FirstOrDefault();
                    var track = TrackDetailsSeralizer.LoadTrackDetails().Where(x => x.TrackId == sessions[i].TrackId.ToString()).FirstOrDefault();
                    var sessionLaps = laps.Where(x => x.SessionId == sessions[i].SessionId).ToList();
                    foreach (var lap in sessionLaps)
                    {
                        writer.Write($"{sessions[i].SessionId},");
                        writer.Write($"{sessions[i].CarId},");
                        writer.Write($"{sessions[i].TrackId},");
                        writer.Write($"{sessions[i].CarClass},");
                        writer.Write($"{sessions[i].CarPi},");
                        writer.Write($"{sessions[i].SessionStart},");
                        writer.Write($"{sessions[i].SessionEnd},");
                        writer.Write($"{sessions[i].SessionEndType},");
                        writer.Write($"{sessions[i].CurrentLapNumber},");
                        writer.Write($"{lap.TimeInSeconds},");
                        writer.Write($"{lap.TimeOfLapTime},");
                        writer.Write($"{lap.LapNumber},");
                        writer.Write($"{lap.IsBestLap},");
                        writer.Write($"{lap.DistanceTravelled},");
                        writer.Write($"{lap.TotalDistanceTravelled},");
                        writer.Write($"{lap.AverageSpeed},");
                        writer.Write($"{lap.FuelRemaining},");
                        writer.Write($"{lap.AvgTyreWear},");
                        writer.Write($"{lap.FlTyreWear},");
                        writer.Write($"{lap.FrTyreWear},");
                        writer.Write($"{lap.RlTyreWear},");
                        writer.Write($"{lap.RrTyreWear},");
                        writer.Write($"{lap.PercentFullThrottle},");
                        writer.Write($"{lap.PercentBrakeApplied},");
                        writer.Write($"{lap.MinSpeed},");
                        writer.Write($"{lap.MaxSpeed},");
                        writer.Write($"{lap.FuelUsed},");
                        writer.Write($"{lap.PercentCoasting},");
                        writer.Write($"{lap.PositionChanges},");
                        writer.Write($"{lap.RacePosition},");
                        writer.Write($"{i} - Car: {car?.YearMakeModel ?? "Unknown"} - Track: {track?.FullTrackName ?? "Unknown"}");
                        writer.WriteLine("");
                    }
                }
            }


        }
    }
}



