using ForzaAnalytics.Controls;
using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.Services.Service;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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
            cUniqueSessions.lMetricValue.Content = svc.UniqueSessions();
            cTotalLaps.lMetricValue.Content = svc.TotalLaps();
            
            cTotalCars.lMetricValue.Content = svc.UniqueCars();
            cTotalTracks.lMetricValue.Content = svc.UniqueCircuits();
            
            cTotalDistance.lMetricValue.Content = $"{svc.TotalDistance()}KM";
            cTotalTimeTracked.lMetricValue.Content = svc.TotalTimeTracked();

            cAverageSessionTime.lMetricValue.Content = svc.AverageSessionTime();
            cAverageSessionDistance.lMetricValue.Content = svc.AverageSessionDistance();

            cAverageSpeed.lMetricValue.Content = Math.Round(svc.AverageSpeed(),2);
            cMaximumSpeed.lMetricValue.Content = svc.MaximumSpeed();

            cAveragePosition.lMetricValue.Content = svc.AveragePosition();
            cTyreChanges.lMetricValue.Content = svc.TotalTyreChanges();

            mPodiums.Update(svc.Wins().ToString(), svc.SecondPlaces().ToString(), svc.ThirdPlaces().ToString());
            cPodiums.Update(svc.Podiums().ToString());

            lvCarByDuration.ItemsSource = svc.TotalTimeTrackedByCar().OrderByDescending(x => x.Duration);
            lvCarByDistance.ItemsSource = svc.TotalDistanceByCar().OrderByDescending(x => x.DistanceTravelled);
            lvCarByPosition.ItemsSource = svc.AveragePositionByCar().OrderBy(x=>double.Parse(x.StatValue));

            lvCarDivisionByDuration.ItemsSource = svc.TotalTimeTrackedByCarDivision().OrderByDescending(x => x.Duration);
            lvCarDivisionByDistance.ItemsSource = svc.TotalDistinceByDivision().OrderByDescending(x => x.DistanceTravelled);


            lvTrackByDuration.ItemsSource = svc.TotalTimeTrackedByTrack().OrderByDescending(x => x.Duration);
            lvTrackByDistance.ItemsSource = svc.TotalDistinceByTrack().OrderByDescending(x => x.DistanceTravelled);
            lvTrackByPosition.ItemsSource = svc.AveragePositionByTrack().OrderBy(x => double.Parse(x.StatValue));


            lvCircuitByDuration.ItemsSource = svc.TotalTimeTrackedByCircuit().OrderByDescending(x => x.Duration);
            lvCircuitByDistance.ItemsSource = svc.TotalDistinceByCircuit().OrderByDescending(x => x.DistanceTravelled);
            lvCircuitByPosition.ItemsSource = svc.AveragePositionByCircuit().OrderBy(x => double.Parse(x.StatValue));


        }
    }
}
