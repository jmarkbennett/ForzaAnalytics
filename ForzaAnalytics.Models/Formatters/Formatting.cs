namespace ForzaAnalytics.Models.Formatters
{
    public static class Formatting
    {
        public static string FormattedTime(float timeInSeconds) // 00:00.0000
        {
            return $"{(int)TimeSpan.FromSeconds(timeInSeconds).Minutes:D2}:{(int)TimeSpan.FromSeconds(timeInSeconds).Seconds:D2}.{(int)TimeSpan.FromSeconds(timeInSeconds).Milliseconds:D3}";
        }

        public static string FormattedPercentage(double val)
        {
            return $"{val.ToString("P2")}";
        }
        public static Enumerators.CarClass GetCarClass(int carClass)
        {
           return (Enumerators.CarClass)carClass;
        }
        public static Enumerators.DriveTrain GetDriveTrain(int driveTrain)
        {
            return (Enumerators.DriveTrain)driveTrain;
        }
    }
}
