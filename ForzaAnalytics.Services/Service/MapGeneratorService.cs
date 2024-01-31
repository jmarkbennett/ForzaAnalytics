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
        public GroupedPositionalData Positions;
        public GroupedPositionalData AllPositions;
        public double InitialCanvasSize { get { return initialCanvasSize; } }
        public float? MinX { get; set; }
        public float? MaxX { get; set; }
        public float? MinZ { get; set; }
        public float? MaxZ { get; set; }
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
            if (IsTracking)
            {
                if (MaxX == null)
                    MaxX = payload.Position.PositionX;
                else
                    if (MaxX < payload.Position.PositionX)
                        MaxX = payload.Position.PositionX;
                if (MaxZ == null)
                    MaxZ = payload.Position.PositionZ;
                else
                    if (MaxZ < payload.Position.PositionZ)
                        MaxZ = payload.Position.PositionZ;

                if (MinX == null)
                    MinX = payload.Position.PositionX;
                else
                    if (MinX > payload.Position.PositionX)
                        MinX = payload.Position.PositionX;
                if (MinZ == null)
                    MinZ = payload.Position.PositionZ;
                else
                    if (MinZ > payload.Position.PositionZ)
                        MinZ = payload.Position.PositionZ;
                }
            if (payload.Race.LapNumber == CurrentLapNumber)
                Positions.Positions.Add(result);
            else
            {
                
                IsTracking = false;
                CurrentLapNumber = payload.Race.LapNumber;
            }
            
            return Positions.GetAdjustedPosition(result);
        }
        public void ResetService()
        {
            CurrentLapNumber = 0;
            MinX = 0;
            MaxX = 0;
            MinZ = 0;
            MaxZ = 0;
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
            switch (option)
            {
                case "Massive (400%)":
                    Positions.MapScale = 4;
                    AllPositions.MapScale = 4;
                    break;
                case "Double (200%)":
                    Positions.MapScale = 2.0;
                    AllPositions.MapScale = 2.0;
                    break;
                case "Half Bigger (150%)":
                    Positions.MapScale = 1.5;
                    AllPositions.MapScale = 1.5;
                    break;
                case "Default (100%)":
                    Positions.MapScale = 1;
                    AllPositions.MapScale = 1;
                    break;
                case "Three Quarters (75%)":
                    Positions.MapScale = 0.75;
                    AllPositions.MapScale = 0.75;
                    break;
                case "Half (50%)":
                    Positions.MapScale = 0.5;
                    AllPositions.MapScale = 0.5;
                    break;
            }
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