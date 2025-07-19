using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.Services.Service;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for LapDetail.xaml
    /// </summary>
    public partial class StatsManager : UserControl
    {
        private StatsService svc;

        public StatsManager()
        {
            InitializeComponent();
            svc = new StatsService();
            GetStats();
        }
        private void GetStats()
        {
            lTotalDistance.Content = $"Distance Driven: {svc.GetTotalDistance()}KM";
            lTotalTracks.Content = $"Circuits Driven: {svc.GetUniqueCircuits()}";
            lTotalCars.Content = $"Cars Driven: {svc.GetUniqueCars()}";
            lTotalLaps.Content = $"Laps Driven: {svc.TotalLaps()}";
            lNumberOfPits.Content = $"Tyre Changes: {svc.TotalTyreChanges()}";
            lAveragePosition.Content = $"Average Position:{svc.AveragePosition()}";
            lWins.Content = $"Wins: {svc.Wins()}";
            lPodiums.Content = $"Podiums: {svc.Podiums()}";
            lUniqueSessions.Content = $"Sessions: {svc.GetSessions()}";
            lAverageSpeed.Content = $"Avg Speed: {Math.Round(svc.AverageSpeed(),2)}KM";
            lMaximumSpeed.Content = $"Max Speed: {Math.Round(svc.MaximumSpeed(),2)}KM";
            lTotalTimeTracked.Content = $"Time Tracked: {svc.TotalTimeTracked()}";

            lvTotalDistanceByCar.ItemsSource = svc.DistanceByCar().OrderByDescending(x=>x.DistanceTravelled);
        }
    }
}
/*
            < Label Grid.Row = "0" Grid.Column = "0" Height = "30" Content = "Unique Sessions"       Name = "lUniqueSessions" ></ Label >
            < Label Grid.Row = "0" Grid.Column = "1" Height = "30" Content = "Total Distance"        Name = "lTotalDistance" ></ Label >

            < Label Grid.Row = "0" Grid.Column = "5" Height = "30" Content = "Total Time Tracked"    Name = "lTotalTimeTracked" ></ Label >
 

            < Label Grid.Row = "1" Grid.Column = "3" Height = "30" Content = "Average Lap Time By Track"     Name = "mAverageLapTimeByTrack" ></ Label >
            < Label Grid.Row = "1" Grid.Column = "4" Height = "30" Content = "Average Session Length"    Name = "lAverageSessionLength" ></ Label >


            < Label Grid.Row = "2" Grid.Column = "0"  Content = "Time Per Track" Name = "lTrackMostDriven" ></ Label >
            < Label Grid.Row = "2" Grid.Column = "3" Content = "Time Per Car" Name = "mTimePerCar" ></ Label >

            < Label Grid.Row = "3" Grid.Column = "0" Content = "Total Distance By Car" Name = "mTotalDistanceByCar" ></ Label >
            < Label Grid.Row = "3" Grid.Column = "3" Content = "Average Fuel Consumption By Car"  Name = "mAverageFuelUseByCar" ></ Label >

*/