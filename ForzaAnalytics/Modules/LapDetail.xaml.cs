using System.Collections.ObjectModel;
using System.Windows.Controls;
using ForzaAnalytics.Models.Core;
using ForzaAnalytics.UdpReader.Model;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for LapDetail.xaml
    /// </summary>
    public partial class LapDetail : UserControl
    {
        public ObservableCollection<LapTime> LapTimes { get; set; }
        private List<double> currentSpeeds;
        private List<double> currentAccelerations;
        public  List<double> currentBrakeApplied;
        private List<double> fuelConsumption;
        private int currentLapNumber;
        public LapDetail()
        {
            LapTimes = [];
            currentSpeeds = [];
            currentAccelerations = [];
            currentBrakeApplied = [];
            fuelConsumption = [];
            currentLapNumber = 0;
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {
            // Lower Rolling Times...
            tbCurrentLapNumber.Content = payload.Race.LapNumber.ToString();
            tbCurrentLapTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.CurrentLapTime);
            tbBestLapTime.Content = Models.Formatters.Formatting.FormattedTime(payload.Race.BestLapTime);
            tbFuel.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Fuel);


            // For this just use the last lap values... and do -1 on the current lap...
            if (LapTimes.Any() && payload.Race.LastLapTime > 0)
            {
                if (LapTimes[LapTimes.Count - 1].TimeInSeconds != payload.Race.LastLapTime)
                {
                    LapTimes.Add(
                        new LapTime()
                        {
                            IsBestLap = (payload.Race.LastLapTime == payload.Race.BestLapTime),
                            LapNumber = payload.Race.LapNumber - 1, // this is debatable..
                            TimeInSeconds = payload.Race.LastLapTime,
                            FuelRemaining = payload.Fuel,
                            AverageSpeed = currentSpeeds.Any() ? currentSpeeds.Average() : 0.0,
                            PercentBrakeApplied = ((double)currentBrakeApplied.Count(x => x > 0) / (double)currentBrakeApplied.Count()),
                            PercentFullThrottle = ((double)currentAccelerations.Count(x => x == 100) / (double)currentBrakeApplied.Count()),
                            MaxSpeed = currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Max() : 0.0,
                            MinSpeed = currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Min() : 0.0,
                            FuelUsed = fuelConsumption.Any() ? fuelConsumption.Max() - fuelConsumption.Min() : 0

                        }
                        );

                    foreach (LapTime t in LapTimes)
                    {
                        t.IsBestLap = (t.TimeInSeconds == payload.Race.BestLapTime);
                    }
                }
            }
            else if (payload.Race.LastLapTime > 0)
            {
                LapTimes.Add(
                    new LapTime()
                    {
                        IsBestLap = (payload.Race.LastLapTime == payload.Race.BestLapTime),
                        LapNumber = payload.Race.LapNumber - 1, // this is debatable..
                        TimeInSeconds = payload.Race.LastLapTime,
                        FuelRemaining = payload.Fuel,
                        AverageSpeed = currentSpeeds.Any() ? currentSpeeds.Average() : 0.0,
                        PercentBrakeApplied = ((double)currentBrakeApplied.Count(x => x > 0) / (double)currentBrakeApplied.Count()),
                        PercentFullThrottle = ((double)currentAccelerations.Count(x => x == 100) / (double)currentBrakeApplied.Count()),
                        MaxSpeed = currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Max() : 0.0,
                        MinSpeed = currentSpeeds.Any() ? currentSpeeds.Where(x => x != 0).Min() : 0.0,
                        FuelUsed = fuelConsumption.Any() ? fuelConsumption.Max() - fuelConsumption.Min() : 0
                    }
                );
            }

            if (currentLapNumber == payload.Race.LapNumber)
            {
                currentAccelerations.Add(payload.Acceleration);
                currentBrakeApplied.Add(payload.Brake);
                currentSpeeds.Add(payload.Speed_Mph);
                fuelConsumption.Add(payload.Fuel);
            }
            else
            {
                currentLapNumber = payload.Race.LapNumber;
                currentSpeeds = [];
                currentAccelerations = [];
                currentBrakeApplied = [];
                fuelConsumption = [];
                currentAccelerations.Add(payload.Acceleration);
                currentBrakeApplied.Add(payload.Brake);
                currentSpeeds.Add(payload.Speed_Mph);
                fuelConsumption.Add(payload.Fuel);
            }

            lvLapTimes.ItemsSource = LapTimes;
        }


        public void ResetEvents()
        {
            tbCurrentLapNumber.Content = string.Empty;
            tbCurrentLapTime.Content = string.Empty;
        }
    }
}
