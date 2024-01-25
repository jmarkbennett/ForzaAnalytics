using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Helpers;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ForzaAnalytics.Models.Formatters;
using ForzaAnalytics.Models.Enumerators;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for PositionMap.xaml
    /// </summary>
    public partial class PositionMap : UserControl
    {
        private bool isTracking = false;
        private GroupedPositionalData mapPositions;
        private GroupedExtendedPositionalData positions;
        private int currentLapNumber = 0;
        private int lapToPlot = -2; // -2 = ALL, -2 = Current, all others are specific Lap
        private bool isRotated = false;
        private double mapScale { get; set; }
        private MapModeOptions mapMode = MapModeOptions.DefaultPosition;

        public PositionMap()
        {
            mapPositions = new GroupedPositionalData();
            positions = new GroupedExtendedPositionalData();
            mapScale = 1.0;
            InitializeComponent();
        }

        public void ReceiveEvents(Telemetry payload)
        {
            tbTrackId.Text = payload.Race.TrackIdentifier.ToString();

            if (isTracking)
            {
                positions.ExtendedPositions.Add(new ExtendedPositionalData(payload.Position.PositionX, payload.Position.PositionY, payload.Position.PositionZ)
                {
                    Acceleration = payload.Acceleration,
                    Brake = payload.Brake,
                    Clutch = payload.Clutch,
                    Handbrake = payload.Handbrake,
                    Speed_Mph = payload.Speed_Mph,
                    RaceTime = payload.Race.CurrentRaceTime,
                    LapTime = payload.Race.CurrentLapTime,
                    LapNumber = payload.Race.LapNumber,
                    GearNumber = payload.GearNumber
                });

                if (currentLapNumber != payload.Race.LapNumber && lapToPlot == -1) // Current Lap and Lap Has Ended...
                {
                    cMapPlot.Children.Clear();
                    ReplotMap();
                    ResizeCanvas();
                }

                if (lapToPlot == -2 || (lapToPlot == -1 && payload.Race.LapNumber == currentLapNumber) || lapToPlot == payload.Race.LapNumber) // if its all/current OR the lap number from the options... show it..
                {
                    Ellipse dot = new Ellipse
                    {
                        Width = 2,
                        Height = 2,
                        Fill = Helpers.ColourHelper.GetColourFromString(Services.Helpers.ColourHelper.GetColourForMapMode(payload, mapMode))
                    };

                    Point canvasPoint = new Point(
                        isRotated ? (payload.Position.PositionZ + positions.ZOffset) * mapScale : (payload.Position.PositionX + positions.XOffset) * mapScale,
                        isRotated ? (payload.Position.PositionX + positions.XOffset) * mapScale : (payload.Position.PositionZ + positions.ZOffset) * mapScale);
                    Canvas.SetLeft(dot, canvasPoint.X);
                    Canvas.SetTop(dot, canvasPoint.Y);
                    cMapPlot.Children.Add(dot);
                }
                if (currentLapNumber != payload.Race.LapNumber)
                    cbLapPoints.Items.Add(payload.Race.LapNumber);
                currentLapNumber = payload.Race.LapNumber;
            }
        }

        public void ResetEvents()
        {
            positions.ResetPositions();
            ReplotPoints();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetEvents();
        }
        private void btnLoadMap_Click(object sender, RoutedEventArgs e)
        {
            LoadMap();
        }

        private bool LoadMap()
        {
            var result = false;
            OpenFileDialog dialog = new();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Load Map";
            if (!string.IsNullOrEmpty(tbTrackId.Text))
                dialog.FileName = $"{tbTrackId.Text}.json";
            if (dialog.ShowDialog() == true)
            {
                mapPositions = MapSerializer.LoadMap(dialog.FileName);
                ReplotPoints();
                if (!string.IsNullOrEmpty(mapPositions.TrackName))
                    tbTrackName.Text = mapPositions.TrackName;
                tbXOffset.Text = mapPositions.XOffset.ToString();
                positions.XOffset = mapPositions.XOffset;
                tbZOffset.Text = mapPositions.ZOffset.ToString();
                positions.ZOffset = mapPositions.ZOffset;
                result = true;
            }
            return result;
        }
        private void tglTrackData_Checked(object sender, RoutedEventArgs e)
        {
            isTracking = true;
            tglTrackData.Content = "Stop Tracking";
        }
        private void tglTrackData_Unchecked(object sender, RoutedEventArgs e)
        {
            isTracking = false;
            tglTrackData.Content = "Start Tracking";
        }
        private void ReplotPoints()
        {
            if (cMapPlot != null)
            {
                cMapPlot.Children.Clear();
                ReplotMap();
                ReplotPositions();
                ResizeCanvas();
            }
        }
        private void ReplotMap()
        {
            var points = new List<Point>();
            // Firstly re-do the map (now as a polygon)
            foreach (var position in mapPositions.GetAdjustedPositions())
            {
                points.Add(new Point(
                    isRotated ? position.Z * mapScale : position.X * mapScale,
                    isRotated ? position.X * mapScale : position.Z * mapScale));
            }
            Polygon polygon = new Polygon();
            polygon.Points = new PointCollection(points);
            polygon.Fill = Brushes.LightGray;
            polygon.Stroke = Brushes.DarkGray;
            polygon.StrokeThickness = 2;

            cMapPlot.Children.Add(polygon);
        }
        private void ReplotPositions()
        {
            // then the other others...
            foreach (var position in positions.GetAdjustedPositions())
            {
                if (lapToPlot == -2 || (lapToPlot == -1 && currentLapNumber == position.LapNumber) || lapToPlot == position.LapNumber)
                {
                    Ellipse dot = new Ellipse
                    {
                        Width = 2,
                        Height = 2,
                        Fill = Helpers.ColourHelper.GetColourFromString(Services.Helpers.ColourHelper.GetColourForMapMode(position, mapMode))
                    };

                    Point canvasPoint = new Point(
                        isRotated ? position.Z * mapScale : position.X * mapScale,
                        isRotated ? position.X * mapScale : position.Z * mapScale);
                    Canvas.SetLeft(dot, canvasPoint.X);
                    Canvas.SetTop(dot, canvasPoint.Y);
                    cMapPlot.Children.Add(dot);
                }
            }
        }
        private void btnApplyOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                positions.XOffset = int.Parse(tbXOffset.Text);
                mapPositions.XOffset = int.Parse(tbXOffset.Text);
            }

            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                positions.ZOffset = int.Parse(tbZOffset.Text);
                mapPositions.ZOffset = int.Parse(tbZOffset.Text);
            }

            ReplotPoints();
        }
        private void ResizeCanvas()
        {
            var tmp = mapPositions.GetAdjustedPositions();
            if (tmp.Any())
            {
                float minX = tmp.Min(x => x.X) ;
                float minZ = tmp.Min(x => x.Z);
                float maxX = tmp.Max(x => x.X);
                float maxZ = tmp.Max(x => x.Z);

                cMapPlot.Height =  isRotated ? (maxX * mapScale) - (minX * mapScale) + 40 : (maxZ * mapScale) - (minZ * mapScale) + 40;
                cMapPlot.Width = isRotated ? (maxZ * mapScale) - (minZ * mapScale) + 40 : (maxX * mapScale) - (minX * mapScale) + 40;
            }
        }
        private void btnReduceMap_Click(object sender, RoutedEventArgs e)
        {
            ReduceMap();
        }
        private void ReduceMap()
        {
            var newPositions = new List<Models.Core.PositionalData>();
            foreach (var row in mapPositions.Positions)
                newPositions.Add(new Models.Core.PositionalData((float)Math.Round(row.X, 0), (float)Math.Round(row.Y, 0), (float)Math.Round(row.Z, 0)));

            mapPositions.Positions = newPositions.Distinct(new PositionalDataComparer()).ToList();
            ReplotPoints();
        }
        private void cbChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var action = (cbChartType.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (action)
            {
                case "Default":
                    mapMode = MapModeOptions.DefaultPosition;
                    break;
                case "Pedal Pressure":
                    mapMode = MapModeOptions.AcceBrake;
                    break;
                case "Speed Heatmap":
                    mapMode = MapModeOptions.SpeedHeatmap;
                    break;
                case "Gear Number":
                    mapMode = MapModeOptions.GearNumber;
                    break;
                default:
                    mapMode = MapModeOptions.DefaultPosition;
                    break;
            }

            ReplotPoints();
        }
        private void cbMapScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var action = (cbMapScale.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (action)
            {
                case "Double (200%)":
                    mapScale = 2.0;
                    break;
                case "Half Bigger (150%)":
                    mapScale = 1.5;
                    break;
                case "Default (100%)":
                    mapScale = 1;
                    break;
                case "Three Quarters (75%)":
                    mapScale = 0.75;
                    break;
                case "Half (50%)":
                    mapScale = 0.5;
                    break;
            }

            ReplotPoints();
        }
        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Save Telemetry";
            dialog.FileName = $"{tbTrackId}_telemetry.json";
            if (dialog.ShowDialog() == true)
            {
                MapSerializer.ExportPositionData(dialog.FileName,positions);
            }
        }
        private void btnImportData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Load Telemtry";
            if (!string.IsNullOrEmpty(tbTrackId.Text))
                dialog.FileName = $"{tbTrackId.Text}.json";
            if (dialog.ShowDialog() == true)
            {
                positions = MapSerializer.LoadPositionData(dialog.FileName);
                positions.XOffset = int.Parse(tbXOffset.Text);
                positions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
                var laps = new List<int>();
                foreach (var position in positions.ExtendedPositions)
                {
                    if (!laps.Contains(position.LapNumber))
                        laps.Add(position.LapNumber);
                }
                foreach(var item in laps)
                    cbLapPoints.Items.Add(item);
            }
        }
        private void tbnRotateMap_Checked(object sender, RoutedEventArgs e)
        {
            isRotated = true;
            ReplotPoints();
        }
        private void tbnRotateMap_Unchecked(object sender, RoutedEventArgs e)
        {
            isRotated = false;
            ReplotPoints();
        }
        private void btnXOffsetDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                tbXOffset.Text = (int.Parse(tbXOffset.Text) - 10).ToString();
                positions.XOffset = int.Parse(tbXOffset.Text);
                mapPositions.XOffset = int.Parse(tbXOffset.Text);
                ReplotPoints();
            }
        }
        private void btnXOffsetUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                tbXOffset.Text = (int.Parse(tbXOffset.Text) + 10).ToString();
                positions.XOffset = int.Parse(tbXOffset.Text);
                mapPositions.XOffset = int.Parse(tbXOffset.Text);
                ReplotPoints();
            }
        }
        private void btnZOffsetDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                tbZOffset.Text = (int.Parse(tbZOffset.Text) - 10).ToString();
                positions.ZOffset = int.Parse(tbZOffset.Text);
                mapPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void btnZOffsetUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                tbZOffset.Text = (int.Parse(tbZOffset.Text) +10).ToString();
                positions.ZOffset = int.Parse(tbZOffset.Text);
                mapPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void cbLapPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var action = (cbLapPoints.SelectedItem as ComboBoxItem)?.Content.ToString();
            if(action == "All Laps")
                lapToPlot = -2;
            else if (action == "Current Lap")
                lapToPlot = -1;
            else if(action == null)
            {
                lapToPlot = (int)cbLapPoints.SelectedItem;
            }
            ReplotPoints();
        }
        private void btnLoadAndReduceMap_Click(object sender, RoutedEventArgs e)
        {
            if (LoadMap())
                ReduceMap();
        }
    }
}
