namespace ForzaAnalytics.Models.Enumerators
{
    public enum CarClass { E, D, C, B, A, S, R, P, X }
    public enum MapModeOptions { DefaultPosition, SpeedHeatmap, GearNumber, AcceBrake }
    public enum DriveTrain { FWD, RWD, AWD }

    public static class GlobalVariables
    {
        public static string[] HeatmapColours = {
            "FF0000",
            "FF8C00",
            "FFA500",
            "DAA520",
            "FFFF00",
            "9ACD32",
            "ADFF2F",
            "7FFF00",
            "32CD32",
            "008000"
        };
        public static string[] GearShades = {
    "000080",
    "0000CD",
    "0000FF",
    "1E90FF",
    "4169E1",
    "6495ED",
    "87CEEB",
    "87CEFA",
    "B0E0E6",
    "F0F8FF"
        };

        public static string[] AccelerationColours =
            {
            "006400",
            "228B22",
            "008000",
            "00FF00",
            "00FF7F"
        };

        public static string[] BrakeColours = {
            "8B0000",
            "B22222",
            "FF0000",
            "DC143C",
            "CD5C5C"
        };

        public static string BaseAccelerationColor = "00AAFF";
        public static string BaseBrakeColour = "FF0000";
        public static string BaseGearNumberColour = "00FF00";
        public static string BaseCoastingColour = "FFFF00";
        public static string BaseSpeedColour = "FFFFFF";
        /* 100%  90%   80%   70%   60%   50%   40%   30%   20 or 10 (Only used with Gear Number) */
        public static string[] PercentColourGrades = ["FF", "DD", "BB", "99", "77", "55", "33", "11", "00"];
    }
    

}
