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
        public static string GetColourForMapMode(ExtendedPositionalData position, MapModeOptions mapMode)
        {
            switch (mapMode)
            {
                case MapModeOptions.DefaultPosition:
                    return "000000";
                case MapModeOptions.AcceBrake:
                    if (position.Acceleration > 90)
                        return "00FF00";
                    else if (position.Acceleration > 75)
                        return "32FF32";
                    else if (position.Acceleration > 50)
                        return "64FF64";
                    else if (position.Acceleration > 25)
                        return "80FF80";
                    else if (position.Brake > 90)
                        return "FF0000";
                    else if (position.Brake > 75)
                        return "FF3232";
                    else if (position.Brake > 50)
                        return "FF6464";
                    else if (position.Brake > 25)
                        return "FF8080";
                    else if (position.Acceleration == 0 && position.Brake == 0)
                        return "FFFF00";
                    break;
                case MapModeOptions.GearNumber:
                    if (position.GearNumber == "1")
                       return "2D86FF";
                    else if (position.GearNumber == "2")
                        return "2774DE";
                    else if (position.GearNumber == "3")
                        return "2367C4";
                    else if (position.GearNumber == "4")
                        return "1E5AAB";
                    else if (position.GearNumber == "5")
                        return "1A4E94";
                    else if (position.GearNumber == "6")
                        return "174380";
                    else if (position.GearNumber == "7")
                        return "13386B";
                    else if (position.GearNumber == "8")
                        return "0F2D57";
                    else if (position.GearNumber == "9")
                        return "0B2140";
                    else if (position.GearNumber == "10")
                        return "050F1C";
                    break;
                default:
                    return "000000";
            }
            return "000000";
        }
        public static string GetColourForMapMode(UdpReader.Model.Telemetry payload, MapModeOptions mapMode)
        {
            switch (mapMode)
            {
                case MapModeOptions.DefaultPosition:
                    return "000000";
                case MapModeOptions.AcceBrake:
                    if (payload.Acceleration > 90)
                        return "00FF00";
                    else if (payload.Acceleration > 75)
                        return "32FF32";
                    else if (payload.Acceleration > 50)
                        return "64FF64";
                    else if (payload.Acceleration > 25)
                        return "80FF80";
                    else if (payload.Brake > 90)
                        return "FF0000";
                    else if (payload.Brake > 75)
                        return "FF3232";
                    else if (payload.Brake > 50)
                        return "FF6464";
                    else if (payload.Brake > 25)
                        return "FF8080";
                    else if (payload.Acceleration == 0 && payload.Brake == 0)
                        return "FFFF00";
                    break;
                case MapModeOptions.GearNumber:
                    if (payload.GearNumber == "1")
                        return "2D86FF";
                    else if (payload.GearNumber == "2")
                        return "2774DE";
                    else if (payload.GearNumber == "3")
                        return "2367C4";
                    else if (payload.GearNumber == "4")
                        return "1E5AAB";
                    else if (payload.GearNumber == "5")
                        return "1A4E94";
                    else if (payload.GearNumber == "6")
                        return "174380";
                    else if (payload.GearNumber == "7")
                        return "13386B";
                    else if (payload.GearNumber == "8")
                        return "0F2D57";
                    else if (payload.GearNumber == "9")
                        return "0B2140";
                    else if (payload.GearNumber == "10")
                        return "050F1C";
                    break;
                default:
                    return "000000";
            }
            return "000000";
        }
    }
}
