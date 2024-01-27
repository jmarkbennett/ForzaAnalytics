using ForzaAnalytics.Models.Core;
using ForzaAnalytics.Models.Helpers;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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
        private TransformGroup mapTransformGroup;
        private double maxSpeed = 0;
        private int currentLapNumber = 0;
        private int lapToPlot = -2; // -2 = ALL, -2 = Current, all others are specific Lap
        private bool isRotated = false;
        private double mapScale = 1.0;
        private MapModeOptions mapMode = MapModeOptions.DefaultPosition;

        public PositionMap()
        {
            mapPositions = new GroupedPositionalData();
            positions = new GroupedExtendedPositionalData();
            InitializeComponent();
            mapTransformGroup = new TransformGroup();
            mapTransformGroup.Children.Add(new RotateTransform(180, 0, 0));
            mapTransformGroup.Children.Add(new ScaleTransform(-1, 1));
        }

        public void ReceiveEvents(Telemetry payload)
        {
            tbTrackId.Text = payload.Race.TrackIdentifier.ToString();
            if (isTracking)
            {
                if (payload.Speed_Mph > maxSpeed)
                    maxSpeed = payload.Speed_Mph;
                positions.ExtendedPositions.Add(new ExtendedPositionalData(payload.Position.PositionX, payload.Position.PositionY, payload.Position.PositionZ)
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
                });

                if (currentLapNumber != payload.Race.LapNumber && lapToPlot == -1) // Current Lap and Lap Has Ended...
                {
                    cMapPlot.Children.Clear();
                    ReplotMap();
                    ResizeCanvas();
                }

                if (lapToPlot == -2 || (lapToPlot == -1 && payload.Race.LapNumber == currentLapNumber) || lapToPlot == payload.Race.LapNumber) // if its all/current OR the lap number from the options... show it..
                {
                    AddPlotPoint(payload.Position.PositionX, payload.Position.PositionZ, ref payload);
                    AddPlotLabels(payload);
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
            maxSpeed = 0;
            var adjustedPositions = positions.GetAdjustedPositions();
            for (var i = 0; i < adjustedPositions.Count; i++)
            {
                if (lapToPlot == -2 || (lapToPlot == -1 && currentLapNumber == adjustedPositions[i].LapNumber) || lapToPlot == adjustedPositions[i].LapNumber)
                {
                    if (adjustedPositions[i].Speed_Mph > maxSpeed)
                        maxSpeed = adjustedPositions[i].Speed_Mph;
                    double prevSpeed = 0;
                    if (i > 10)
                        prevSpeed = adjustedPositions[i - 10].Speed_Mps;
                    AddPlotPoint(adjustedPositions[i].X, adjustedPositions[i].Z, adjustedPositions[i], prevSpeed);
                    AddPlotLabels(i > 0, adjustedPositions[i], i > 0 ? adjustedPositions[i - 1] : null);
                }
            }
        }
        private Label GeneratePlotLabel(double x, double z, string content)
        {
            double width = 30;
            double height = 30;
            if (mapMode == MapModeOptions.SpeedHeatmap)
                width = 80;
            Label label = new Label()
            {
                Content = content,
                FontSize = 16,
                Width = width,
                Height = height,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                RenderTransform = mapTransformGroup,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(1),
                Margin =
                new Thickness(
                        isRotated ? z * mapScale : x * mapScale,
                        isRotated ? x * mapScale : z * mapScale,
                        0.0,
                        0.0
                    )
            };

            return label;
        }
        private void AddPlotPoint(double x, double z, ExtendedPositionalData data, double? prevSpeed)
        {
            Ellipse dot = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Helpers.ColourHelper.GetColourFromString(Services.Helpers.ColourHelper.GetColourForMapMode(data, mapMode, maxSpeed, prevSpeed)),
            };

            Point canvasPoint = new Point(
                isRotated ? z * mapScale : x * mapScale,
                isRotated ? x * mapScale : z * mapScale);
            Canvas.SetLeft(dot, canvasPoint.X);
            Canvas.SetTop(dot, canvasPoint.Y);
            cMapPlot.Children.Add(dot);
        }
        private void AddPlotPoint(double x, double z, ref Telemetry data)
        {
            double prevSpeed = 0;
            if(positions.ExtendedPositions != null && positions.ExtendedPositions.Count > 10)
                prevSpeed = positions.ExtendedPositions[positions.ExtendedPositions.Count -10].Speed_Mps;
            Ellipse dot = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Helpers.ColourHelper.GetColourFromString(Services.Helpers.ColourHelper.GetColourForMapMode(data, mapMode, maxSpeed, prevSpeed)),
            };

            Point canvasPoint = new Point(
                isRotated ? (z + positions.ZOffset) * mapScale : (x + positions.XOffset) * mapScale,
                isRotated ? (x + positions.XOffset) * mapScale : (z + positions.ZOffset) * mapScale);
            Canvas.SetLeft(dot, canvasPoint.X);
            Canvas.SetTop(dot, canvasPoint.Y);
            cMapPlot.Children.Add(dot);
        }
        private void AddPlotLabels(Telemetry currentRow)
        {
            switch (mapMode)
            {
                case MapModeOptions.GearNumber:
                    if (
                        positions.ExtendedPositions == null ||
                        positions.ExtendedPositions.Count == 0 ||
                        (
                            positions.ExtendedPositions.Count > 2 
                            && 
                            currentRow.GearNumber != positions.ExtendedPositions[positions.ExtendedPositions.Count - 2].GearNumber
                        ))
                        cMapPlot.Children.Add(
                            GeneratePlotLabel(
                                currentRow.Position.PositionX + positions.XOffset, 
                                currentRow.Position.PositionZ + positions.ZOffset, 
                                currentRow.GearNumber));
                    break;
            }
        }
        private void AddPlotLabels(bool hasPrevious, ExtendedPositionalData currentRow, ExtendedPositionalData? previousRow = null)
        {
            switch (mapMode)
            {
                case MapModeOptions.GearNumber:
                    if (!hasPrevious || currentRow.GearNumber != previousRow.GearNumber)
                        cMapPlot.Children.Add(GeneratePlotLabel(currentRow.X, currentRow.Z, currentRow.GearNumber));
                    break;
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
                float minX = tmp.Min(x => x.X);
                float minZ = tmp.Min(x => x.Z);
                float maxX = tmp.Max(x => x.X);
                float maxZ = tmp.Max(x => x.Z);

                cMapPlot.Height = isRotated ? (maxX * mapScale) - (minX * mapScale) + 40 : (maxZ * mapScale) - (minZ * mapScale) + 40;
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
                case "Acceleration":
                    mapMode = MapModeOptions.Acceleration;
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
                case "Massive (400%)":
                    mapScale = 4.0;
                    break;
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
            dialog.FileName = $"{tbTrackId.Text}_telemetry.json";
            if (dialog.ShowDialog() == true)
            {
                MapSerializer.ExportPositionData(dialog.FileName, positions);
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
                foreach (var item in laps)
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
                tbZOffset.Text = (int.Parse(tbZOffset.Text) + 10).ToString();
                positions.ZOffset = int.Parse(tbZOffset.Text);
                mapPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void cbLapPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var action = (cbLapPoints.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (action == "All Laps")
                lapToPlot = -2;
            else if (action == "Current Lap")
                lapToPlot = -1;
            else if (action == null)
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
