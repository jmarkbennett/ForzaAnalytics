using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Enumerators;
namespace ForzaAnalytics.Services.Helpers
{
    public static class ColourHelper
    {
        public static string GetCarClassColour(CarClass carClass)
        {
            switch (carClass)
            {
                case CarClass.E:
                    return "C7368E";
                case CarClass.D:
                    return "127F85";
                case CarClass.C:
                    return "BB7A00";
                case CarClass.B:
                    return "C54E00";
                case CarClass.A:
                    return "992800";
                case CarClass.S:
                    return "613BBF";
                case CarClass.R:
                    return "025BB6";
                case CarClass.P:
                    return "0D853F";
                case CarClass.X:
                    return "405B42";
            }
            return "FFFFFF";
        }
        public static string GetColourForMapMode(ExtendedPositionalData position, MapModeOptions mapMode, double? maxSpeed = null, double? prevSpeed = null)
        {
            switch (mapMode)
            {
                case MapModeOptions.DefaultPosition:
                    return "000000";
                case MapModeOptions.AcceBrake:
                    if (position.Acceleration > 90)
                        return GlobalVariables.AccelerationColours[4];
                    else if (position.Acceleration > 80)
                        return GlobalVariables.AccelerationColours[3];
                    else if (position.Acceleration > 60)
                        return GlobalVariables.AccelerationColours[2];
                    else if (position.Acceleration > 40)
                        return GlobalVariables.AccelerationColours[1];
                    else if (position.Acceleration > 20)
                        return GlobalVariables.AccelerationColours[0];
                    if (position.Brake > 90)
                        return GlobalVariables.BrakeColours[4];
                    else if (position.Brake > 80)
                        return GlobalVariables.BrakeColours[3];
                    else if (position.Brake > 60)
                        return GlobalVariables.BrakeColours[2];
                    else if (position.Brake > 40)
                        return GlobalVariables.BrakeColours[1];
                    else if (position.Brake > 20)
                        return GlobalVariables.BrakeColours[0];
                    else if (position.Acceleration == 0 && position.Brake == 0)
                        return GlobalVariables.BaseCoastingColour;
                    break;
                case MapModeOptions.GearNumber:
                    if (position.GearNumber == "R")
                        return "FF0000";
                    else if (position.GearNumber != "N")
                        return GlobalVariables.GearShades[int.Parse(position.GearNumber)];
                    break;
                case MapModeOptions.SpeedHeatmap:
                    if (position.Speed_Mps > (maxSpeed * 0.9))
                        return GlobalVariables.HeatmapColours[9];
                    else if (position.Speed_Mps > (maxSpeed * 0.8))
                        return GlobalVariables.HeatmapColours[8];
                    else if (position.Speed_Mps > (maxSpeed * 0.7))
                        return GlobalVariables.HeatmapColours[7];
                    else if (position.Speed_Mps > (maxSpeed * 0.6))
                        return GlobalVariables.HeatmapColours[6];
                    else if (position.Speed_Mps > (maxSpeed * 0.5))
                        return GlobalVariables.HeatmapColours[5];
                    else if (position.Speed_Mps > (maxSpeed * 0.4))
                        return GlobalVariables.HeatmapColours[4];
                    else if (position.Speed_Mps > (maxSpeed * 0.3))
                        return GlobalVariables.HeatmapColours[3];
                    else if (position.Speed_Mps > (maxSpeed * 0.2))
                        return GlobalVariables.HeatmapColours[2];
                    else if (position.Speed_Mps > (maxSpeed * 0.1))
                        return GlobalVariables.HeatmapColours[1];
                    break;
                case MapModeOptions.Acceleration:
                    if (prevSpeed == null)
                        return "0000FF";
                    else if (Math.Round(position.Speed_Mps, 3) > Math.Round(prevSpeed.Value, 3)) // Accelerating
                        return "00FF00";
                    else if (Math.Round(position.Speed_Mps, 3) == Math.Round(prevSpeed.Value, 3)) // Maintaining
                        return "0000FF";
                    else if (Math.Round(position.Speed_Mps, 3) < Math.Round(prevSpeed.Value, 3)) // Slowing
                        return "FF0000";
                    break;
                default:
                    return "000000";
            }
            return "000000";
        }
        public static string GetColourForMapMode(UdpReader.Model.Telemetry position, MapModeOptions mapMode, double? maxSpeed = null, double? prevSpeed = null)
        {
            switch (mapMode)
            {
                case MapModeOptions.DefaultPosition:
                    return "000000";
                case MapModeOptions.AcceBrake:
                    if (position.Acceleration > 90)
                        return GlobalVariables.AccelerationColours[4];
                    else if (position.Acceleration > 80)
                        return GlobalVariables.AccelerationColours[3];
                    else if (position.Acceleration > 60)
                        return GlobalVariables.AccelerationColours[2];
                    else if (position.Acceleration > 40)
                        return GlobalVariables.AccelerationColours[1];
                    else if (position.Acceleration > 20)
                        return GlobalVariables.AccelerationColours[0];
                    if (position.Brake > 90)
                        return GlobalVariables.BrakeColours[4];
                    else if (position.Brake > 80)
                        return GlobalVariables.BrakeColours[3];
                    else if (position.Brake > 60)
                        return GlobalVariables.BrakeColours[2];
                    else if (position.Brake > 40)
                        return GlobalVariables.BrakeColours[1];
                    else if (position.Brake > 20)
                        return GlobalVariables.BrakeColours[0];
                    else if (position.Acceleration == 0 && position.Brake == 0)
                        return GlobalVariables.BaseCoastingColour;
                    break;
                case MapModeOptions.GearNumber:
                    if (position.GearNumber == "R")
                        return "FF0000";
                    else if (position.GearNumber != "N")
                        return GlobalVariables.GearShades[int.Parse(position.GearNumber)];
                    break;
                case MapModeOptions.SpeedHeatmap:
                    if (position.Speed_Mph > (maxSpeed * 0.9))
                        return GlobalVariables.HeatmapColours[9];
                    else if (position.Speed_Mph > (maxSpeed * 0.8))
                        return GlobalVariables.HeatmapColours[8];
                    else if (position.Speed_Mph > (maxSpeed * 0.7))
                        return GlobalVariables.HeatmapColours[7];
                    else if (position.Speed_Mph > (maxSpeed * 0.6))
                        return GlobalVariables.HeatmapColours[6];
                    else if (position.Speed_Mph > (maxSpeed * 0.5))
                        return GlobalVariables.HeatmapColours[5];
                    else if (position.Speed_Mph > (maxSpeed * 0.4))
                        return GlobalVariables.HeatmapColours[4];
                    else if (position.Speed_Mph > (maxSpeed * 0.3))
                        return GlobalVariables.HeatmapColours[3];
                    else if (position.Speed_Mph > (maxSpeed * 0.2))
                        return GlobalVariables.HeatmapColours[2];
                    else if (position.Speed_Mph > (maxSpeed * 0.1))
                        return GlobalVariables.HeatmapColours[1];
                    break;
                case MapModeOptions.Acceleration:
                    if (prevSpeed == null)
                        return "0000FF";
                    else if (Math.Round(position.Speed_Mps,3) > Math.Round(prevSpeed.Value,3)) // Accelerating
                        return "00FF00";
                    else if (Math.Round(position.Speed_Mps,3)  == Math.Round(prevSpeed.Value,3)) // Maintaining
                        return "0000FF";
                    else if (Math.Round(position.Speed_Mps,3) < Math.Round(prevSpeed.Value,3)) // Slowing
                        return "FF0000";
                    break;
                default:
                    return "000000";
            }
            return "000000";
        }
    }
}
