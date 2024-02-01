using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Helpers;
using ForzaAnalytics.Services.Helpers;
using ForzaAnalytics.UdpReader.Model;

namespace ForzaAnalytics.Services.Service
{
    public class MapGeneratorService
    {
        private const double initialCanvasSize = 8000;
        public int CurrentLapNumber;
        public bool IsTracking = false;
        public bool HasLapChanged = false;
        public GroupedPositionalData Positions;
        public GroupedPositionalData AllPositions;
        public double InitialCanvasSize { get { return initialCanvasSize; } }
        public float? MaxX { get; set; }
        public float? MinZ { get; set; }
        public MapGeneratorService()
        {
            CurrentLapNumber = 0;
            Positions = new GroupedPositionalData();
            AllPositions = new GroupedPositionalData();
        }
        public Models.Core.PositionalData Update(Telemetry payload)
        {
            var result = new Models.Core.PositionalData(
                payload.Position.PositionX,
                payload.Position.PositionY,
                payload.Position.PositionZ
            );
            Positions.TrackId = payload.Race.TrackIdentifier;
            AllPositions.TrackId = payload.Race.TrackIdentifier;
            if (MaxX == null)
                MaxX = payload.Position.PositionX;
            else
                if (MaxX < payload.Position.PositionX)
                MaxX = payload.Position.PositionX;
            if (MinZ == null)
                MinZ = payload.Position.PositionZ;
            else
                if (MinZ > payload.Position.PositionZ)
                MinZ = payload.Position.PositionZ;
            
            if (payload.Race.LapNumber == CurrentLapNumber)
                Positions.Positions.Add(result);
            else
            {
                if (Positions.Positions.Count > 1)
                {
                    IsTracking = false;
                    CurrentLapNumber = payload.Race.LapNumber;
                }
                else
                {
                    CurrentLapNumber = payload.Race.LapNumber;
                }
            }

            return result;
        }
        public void ResetService()
        {
            CurrentLapNumber = 0;
            MaxX = 0;
            MinZ = 0;
            AllPositions = new GroupedPositionalData();
            Positions = new GroupedPositionalData();
        }
        public void UpdateTrackName(string trackName)
        {
            if (!string.IsNullOrEmpty(trackName))
            {
                Positions.TrackName = trackName;
                AllPositions.TrackName = trackName;
            }
        }
        public void ReduceMap(int precision)
        {
            var newPositions = new List<Models.Core.PositionalData>();
            foreach (var row in AllPositions.Positions)
                newPositions.Add(
                    new Models.Core.PositionalData(
                        (float)Math.Round(row.X, precision),
                        (float)Math.Round(row.Y, precision),
                        (float)Math.Round(row.Z, precision)
                        )
                );
            AllPositions.Positions = newPositions.Distinct(new PositionalDataComparer()).ToList();
            newPositions = new List<Models.Core.PositionalData>();
            foreach (var row in Positions.Positions)
                newPositions.Add(
                    new Models.Core.PositionalData(
                        (float)Math.Round(row.X, precision),
                        (float)Math.Round(row.Y, precision),
                        (float)Math.Round(row.Z, precision)
                        )
                );
            Positions.Positions = newPositions.Distinct(new PositionalDataComparer()).ToList();
        }
        public int CommitPositions()
        {
            if (Positions.Positions.Count > 0)
            {
                var rows = Positions.Positions.Count;
                AllPositions.Positions.AddRange(Positions.Positions);
                Positions.ResetPositions();
                return rows;
            }
            else
                return 0;
        }
        public void SetMapScale(string option)
        {
            double scale = double.Parse(option.Replace("%", ""));
            Positions.MapScale = scale / 100.0;
            AllPositions.MapScale = scale / 100.0;
        }
        public void HandleMapPan(int deltaX, int deltaZ)
        {
            Positions.XOffset = Positions.XOffset - deltaX;
            Positions.ZOffset = Positions.ZOffset + deltaZ;
            AllPositions.XOffset = AllPositions.XOffset - deltaX;
            AllPositions.ZOffset = AllPositions.ZOffset + deltaZ;
        }
    }
}