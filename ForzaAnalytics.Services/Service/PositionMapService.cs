using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Enumerators;
using ForzaAnalytics.Models.Helpers;
using ForzaAnalytics.Services.Helpers;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Services.Service
{
    public class PositionMapService
    {
        public bool IsTracking = false;
        public GroupedPositionalData MapPositions;
        public GroupedExtendedPositionalData Positions;

        public double MaxSpeed = 0;
        public int CurrentLapNumber = 0;
        public int LapToPlot = -2; // -2 = ALL, -2 = Current, all others are specific Lap
        public MapModeOptions MapMode = MapModeOptions.DefaultPosition;

        public PositionMapService()
        {
            MapPositions = new GroupedPositionalData();
            Positions = new GroupedExtendedPositionalData();
        }
        public bool IsPlottedLap(int payloadLap)
        {
            return LapToPlot == -2 || (LapToPlot == -1 && payloadLap == CurrentLapNumber) || LapToPlot == payloadLap;
        }
        public bool CurrentLapEnded(int payloadLap)
        {
            return CurrentLapNumber != payloadLap && LapToPlot == -1;
        }
        public bool SetupNewSession(Telemetry payload)
        {
            var isNew = false;
            if (Positions.TrackId == -1)
                Positions.TrackId = payload.Race.TrackIdentifier;
            else if (Positions.TrackId != payload.Race.TrackIdentifier)
            {
                isNew = true;
                ResetService();
                Positions.TrackId = payload.Race.TrackIdentifier;
            }
            return isNew;
        }
        public ExtendedPositionalData Update(Telemetry payload)
        {
            if (payload.Speed_Mps > MaxSpeed)
                MaxSpeed = payload.Speed_Mps;

            var result = new ExtendedPositionalData(
            payload.Position.PositionX,
            payload.Position.PositionY,
            payload.Position.PositionZ)
            {
                Acceleration = payload.Acceleration,
                Brake = payload.Brake,
                Clutch = payload.Clutch,
                Handbrake = payload.Handbrake,
                Speed_Mph = payload.Speed_Mph,
                Speed_Mps = payload.Speed_Mps,
                RaceTime = payload.Race.CurrentRaceTime,
                LapTime = payload.Race.CurrentLapTime,
                LapNumber = payload.Race.LapNumber,
                GearNumber = payload.GearNumber
            };
            Positions.ExtendedPositions.Add(result);
            return result;
        }
        public ExtendedPositionalData Replay(ExtendedPositionalData row)
        {
            if (row.Speed_Mps > MaxSpeed)
                MaxSpeed = row.Speed_Mps;

            return row;
        }
        public void ResetService()
        {
            Positions.ResetPositions();
            MapPositions.ResetPositions();
            MaxSpeed = 0;
            CurrentLapNumber = 0;
        }
        public void LoadMap(string filename)
        {
            MapPositions = MapSerializer.LoadMap(filename);
            Positions.XOffset = MapPositions.XOffset;
            Positions.ZOffset = MapPositions.ZOffset;
            if (MapPositions.MapScale != null)
                Positions.MapScale = MapPositions.MapScale;
        }
        public double GetPreviousSpeed()
        {
            double result = 0;
            if (Positions.ExtendedPositions != null && Positions.ExtendedPositions.Count > 10)
                result = Positions.ExtendedPositions[Positions.ExtendedPositions.Count - 10].Speed_Mps;

            return result;
        }
        public double GetPreviousSpeed(int ordinal)
        {
            double result = 0;
            if (Positions.ExtendedPositions != null && Positions.ExtendedPositions.Count > 10 && ordinal > 10)
                result = Positions.ExtendedPositions[ordinal - 10].Speed_Mps;

            return result;
        }
        public bool HasGearNumberChanged(Telemetry currentRow)
        {
            if (
                 Positions.ExtendedPositions == null ||
                 Positions.ExtendedPositions.Count == 0 ||
                 (
                  Positions.ExtendedPositions.Count > 2 &&
                  currentRow.GearNumber != Positions.ExtendedPositions[Positions.ExtendedPositions.Count - 2].GearNumber
                 )
                )
                return true;
            return false;
        }
        public void ReduceMap()
        {
            var newPositions = new List<Models.Core.PositionalData>();
            foreach (var row in MapPositions.Positions)
                newPositions.Add(new Models.Core.PositionalData((float)Math.Round(row.X, 0), (float)Math.Round(row.Y, 0), (float)Math.Round(row.Z, 0)));
            MapPositions.Positions = newPositions.Distinct(new PositionalDataComparer()).ToList();
        }
        public void SetChartType(string option)
        {
            switch (option)
            {
                case "Default":
                    MapMode = MapModeOptions.DefaultPosition;
                    break;
                case "Pedal Pressure":
                    MapMode = MapModeOptions.AcceBrake;
                    break;
                case "Speed Heatmap":
                    MapMode = MapModeOptions.SpeedHeatmap;
                    break;
                case "Gear Number":
                    MapMode = MapModeOptions.GearNumber;
                    break;
                case "Acceleration":
                    MapMode = MapModeOptions.Acceleration;
                    break;
                default:
                    MapMode = MapModeOptions.DefaultPosition;
                    break;
            }
        }
        public void SetMapScale(string option)
        {
            double scale = double.Parse(option.Replace("%", ""));
            Positions.MapScale = scale / 100.0;
            MapPositions.MapScale = scale / 100.0;
        }
        public void ImportTelemetry(string filename)
        {
            Positions = MapSerializer.LoadPositionData(filename);
            Positions.MapScale = MapPositions.MapScale;
            if (MapPositions != null)
            {
                Positions.ZOffset = MapPositions.ZOffset;
                Positions.XOffset = MapPositions.XOffset;
            }
        }
        public void SetLapsToPlot(string option)
        {
            if (option == "All Laps")
                LapToPlot = -2;
            else if (option == "Current Lap")
                LapToPlot = -1;
            else if (option != null)
            {
                LapToPlot = int.Parse(option);
            }
        }
        public void HandleMapPan(int deltaX, int deltaZ)
        {
            MapPositions.XOffset = MapPositions.IsRotated ? MapPositions.XOffset + deltaZ : MapPositions.XOffset - deltaX;
            MapPositions.ZOffset = MapPositions.IsRotated ? MapPositions.ZOffset - deltaX : MapPositions.ZOffset + deltaZ;
            Positions.XOffset = Positions.IsRotated ? Positions.XOffset + deltaZ : Positions.XOffset - deltaX;
            Positions.ZOffset = Positions.IsRotated ? Positions.ZOffset - deltaX : Positions.ZOffset + deltaZ;
        }
        public float GetSuggestedXOffset()
        {
            return MapPositions.Positions.Max(x => x.X) + 40;
        }
        public float GetSuggestedZOffset()
        {
            return MapPositions.Positions.Min(x => x.Z) - 40;
        }

    }
}
