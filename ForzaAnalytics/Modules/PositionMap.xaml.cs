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
using System.Windows.Input;
using ForzaAnalytics.Services.Service;
using ForzaAnalytics.Services.Helpers;
using System.Windows.Media.Media3D;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Windows.Threading;
namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for PositionMap.xaml
    /// </summary>
    public partial class PositionMap : UserControl
    {
        private bool mousePressed = false;
        private bool wasDragged = false;
        private double mousePressedInitialX = 0;
        private double mousePressedInitialY = 0;
        private TransformGroup mapTransformGroup;
        private PositionMapService svc;
        private int CurrentOrdinal = 0;
        private DispatcherTimer replayTimer;
        private bool isReplaying = false;
        public PositionMap()
        {
            svc = new PositionMapService();
            InitializeComponent();
            mapTransformGroup = new TransformGroup();
            mapTransformGroup.Children.Add(new RotateTransform(180, 0, 0));
            mapTransformGroup.Children.Add(new ScaleTransform(-1, 1));
        }

        #region Receive, Reset Start & Stop
        public void ReceiveEvents(Telemetry payload)
        {
            tbTrackId.Text = payload.Race.TrackIdentifier.ToString();
            if (svc.IsTracking)
            {
                var isNewSession = svc.SetupNewSession(payload);
 
                var position = svc.Update(payload);
                if (svc.CurrentLapEnded(payload.Race.LapNumber)) // Current Lap and Lap Has Ended...
                {
                    cMapPlot.Children.Clear();
                    ReplotMap();
                }

                if (svc.IsPlottedLap(payload.Race.LapNumber)) // if its all/current OR the lap number from the options... show it..
                {
                    AddCanvasPoint(position);
                    AddPlotLabels(payload);
                }
                AddLapsToDropDown(ref payload);
                svc.CurrentLapNumber = payload.Race.LapNumber;
                miExport.IsEnabled = true;
            }
        }
        public void ResetEvents()
        {
            svc.ResetService();
            tbTrackId.Text = string.Empty;
            tbTrackName.Text = string.Empty;
            ReplotPoints();
        }
        private void miResetAll_Click(object sender, RoutedEventArgs e)
        {
            ResetEvents();
            miExport.IsEnabled = false;
            miResetAll.IsEnabled = false;
        }
        private void miTrackData_Toggle(object sender, RoutedEventArgs e)
        {
            if (miIsListening.IsChecked)
            {
                svc.IsTracking = true;
                btnReplayData.IsEnabled = false;
                eIsListening.Fill = new SolidColorBrush(Colors.YellowGreen);
            }
            else
            {
                svc.IsTracking = false;
                eIsListening.Fill = new SolidColorBrush(Colors.Red);
            }
        }
        #endregion

        #region Re-plot Map Data
        private void ReplotPoints()
        {
            if (cMapPlot != null)
            {
                cMapPlot.Children.Clear();
                ReplotMap();
                ReplotPositions();
            }
        }
        private void ReplotMap()
        {
            var points = new List<Point>();

            foreach (var position in svc.MapPositions.Positions)
                points.Add(
                    new Point(
                        svc.MapPositions.GetAdjustedXCoordinate(position.X, position.Z),
                        svc.MapPositions.GetAdjustedZCoordinate(position.X, position.Z)
                    )
                );

            Polygon polygon = new Polygon();
            polygon.Points = new PointCollection(points);
            polygon.Fill = Brushes.LightGray;
            polygon.Stroke = Brushes.DarkGray;
            polygon.StrokeThickness = 2;

            cMapPlot.Children.Add(polygon);
        }
        private void ReplotPositions()
        {
            svc.MaxSpeed = 0;
            var positions = svc.Positions.ExtendedPositions;
            for (var i = 0; i < (isReplaying ? CurrentOrdinal : positions.Count); i++)
            {
                if (svc.IsPlottedLap(positions[i].LapNumber))
                {
                    if (positions[i].Speed_Mps > svc.MaxSpeed)
                        svc.MaxSpeed = positions[i].Speed_Mps;
                    double prevSpeed = 0;
                    if (i > 10)
                        prevSpeed = positions[i - 10].Speed_Mps;
                    AddCanvasPoint(positions[i], prevSpeed);
                    AddPlotLabels(i > 0, positions[i], i > 0 ? positions[i - 1] : null);
                    if (i < (positions.Count - 1))
                        AddLapTimePlotLabels(true, positions[i], positions[i + 1]);
                }
            }
        }
        #endregion

        #region Plot Labels
        private Label GeneratePlotLabel(double x, double z, string content)
        {
            double width = 30;
            double height = 30;
            if (svc.MapMode == MapModeOptions.SpeedHeatmap)
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
                        svc.Positions.GetAdjustedXCoordinate((float)x, (float)z),
                        svc.Positions.GetAdjustedZCoordinate((float)x, (float)z),
                        0.0,
                        0.0
                    )
            };

            return label;
        }
        private Label GenerateLapTimePlotLabel(double x, double z, int lapNumber, float lapTime)
        {
            double width = 120;
            double height = 30;
            if (svc.MapMode == MapModeOptions.SpeedHeatmap)
                width = 80;
            Label label = new Label()
            {
                Content = $"{lapNumber}|{Models.Formatters.Formatting.FormattedTime(lapTime)}",
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
                        svc.Positions.GetAdjustedXCoordinate((float)x, (float)z),
                        svc.Positions.GetAdjustedZCoordinate((float)x, (float)z),
                        0.0,
                        0.0
                    )
            };

            return label;
        }
        private void AddPlotLabels(Telemetry currentRow)
        {
            if (svc.MapMode == MapModeOptions.GearNumber || miShowGearChanges.IsChecked)
                if (svc.HasGearNumberChanged(currentRow))
                {
                    cMapPlot.Children.Add(
                        GeneratePlotLabel(
                            currentRow.Position.PositionX,
                            currentRow.Position.PositionZ,
                            currentRow.GearNumber)
                    );
                }
        }
        private void AddPlotLabels(bool hasPrevious, ExtendedPositionalData currentRow, ExtendedPositionalData? previousRow = null)
        {
            if (svc.MapMode == MapModeOptions.GearNumber || miShowGearChanges.IsChecked)
                if (!hasPrevious || currentRow.GearNumber != previousRow.GearNumber)
                    cMapPlot.Children.Add(GeneratePlotLabel(currentRow.X, currentRow.Z, currentRow.GearNumber));
        }
        private void AddLapTimePlotLabels(bool hasNext, ExtendedPositionalData currentRow, ExtendedPositionalData? nextRow = null)
        {
            if (miShowLapTimes.IsChecked)
                if (hasNext && currentRow.LapNumber != nextRow.LapNumber)
                {
                    cMapPlot.Children.Add(GenerateLapTimePlotLabel(currentRow.X, currentRow.Z, currentRow.LapNumber, currentRow.LapTime));
                }
        }
        private void AddLapTimePlotLabels(Telemetry currentRow)
        {
            if (miShowLapTimes.IsChecked)
            {
                cMapPlot.Children.Add(GenerateLapTimePlotLabel(currentRow.Position.PositionX, currentRow.Position.PositionZ, currentRow.Race.LapNumber - 1, currentRow.Race.LastLapTime));
            }
        }
        #endregion

        #region Add Canvas Points (lines)
        private void AddCanvasPoint(ExtendedPositionalData position, double? prevSpeed)
        {
            Ellipse dot = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Helpers.ColourHelper.GetColourFromString(ColourHelper.GetColourForMapMode(position, svc.MapMode, svc.MaxSpeed, prevSpeed))
            };

            Point canvasPoint = new Point(
                svc.MapPositions.GetAdjustedXCoordinate(position.X, position.Z),
                svc.MapPositions.GetAdjustedZCoordinate(position.X, position.Z)
            );

            Canvas.SetLeft(dot, canvasPoint.X);
            Canvas.SetTop(dot, canvasPoint.Y);
            cMapPlot.Children.Add(dot);
        }
        private void AddCanvasPoint(ExtendedPositionalData position)
        {
            double prevSpeed = isReplaying ? svc.GetPreviousSpeed(CurrentOrdinal) : svc.GetPreviousSpeed();
            Ellipse dot = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Helpers.ColourHelper.GetColourFromString(ColourHelper.GetColourForMapMode(position, svc.MapMode, svc.MaxSpeed, prevSpeed)),
            };

            Point canvasPoint = new Point(
                svc.MapPositions.GetAdjustedXCoordinate(position.X, position.Z),
                svc.MapPositions.GetAdjustedZCoordinate(position.X, position.Z)
            );
            Canvas.SetLeft(dot, canvasPoint.X);
            Canvas.SetTop(dot, canvasPoint.Y);
            cMapPlot.Children.Add(dot);
        }
        #endregion

        #region Handle Offsets
        private void miShowOffsets_Click(object sender, RoutedEventArgs e)
        {
            if (miShowOffsets.IsChecked)
                gOffsets.Visibility = Visibility.Visible;
            else
                gOffsets.Visibility = Visibility.Collapsed;
        }
        private void btnApplyOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                svc.Positions.XOffset = int.Parse(tbXOffset.Text);
                svc.MapPositions.XOffset = int.Parse(tbXOffset.Text);
            }

            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                svc.Positions.ZOffset = int.Parse(tbZOffset.Text);
                svc.MapPositions.ZOffset = int.Parse(tbZOffset.Text);
            }

            ReplotPoints();
        }
        private void btnXOffsetDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                tbXOffset.Text = (int.Parse(tbXOffset.Text) - 10).ToString();
                svc.Positions.XOffset = int.Parse(tbXOffset.Text);
                svc.MapPositions.XOffset = int.Parse(tbXOffset.Text);
                ReplotPoints();
            }
        }
        private void btnXOffsetUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                tbXOffset.Text = (int.Parse(tbXOffset.Text) + 10).ToString();
                svc.Positions.XOffset = int.Parse(tbXOffset.Text);
                svc.MapPositions.XOffset = int.Parse(tbXOffset.Text);
                ReplotPoints();
            }
        }
        private void btnZOffsetDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                tbZOffset.Text = (int.Parse(tbZOffset.Text) - 10).ToString();
                svc.Positions.ZOffset = int.Parse(tbZOffset.Text);
                svc.MapPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void btnZOffsetUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                tbZOffset.Text = (int.Parse(tbZOffset.Text) + 10).ToString();
                svc.Positions.ZOffset = int.Parse(tbZOffset.Text);
                svc.MapPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void btnSuggestOffset_Click(object sender, RoutedEventArgs e)
        {
            svc.Positions.XOffset = (int)svc.GetSuggestedXOffset();
            svc.MapPositions.XOffset = (int)svc.GetSuggestedXOffset();
            svc.Positions.ZOffset = (int)svc.GetSuggestedZOffset();
            svc.MapPositions.ZOffset = (int)svc.GetSuggestedZOffset();
            tbXOffset.Text = svc.MapPositions.XOffset.ToString();
            tbZOffset.Text = svc.MapPositions.ZOffset.ToString();
            ReplotPoints();
        }
        #endregion

        #region Generic Map Actions
        private void miLoadMap_Click(object sender, RoutedEventArgs e)
        {
            LoadMap();
        }
        private bool LoadMap()
        {
            var result = false;
            OpenFileDialog dialog = new();
            dialog.Filter = "FZMAP files (*.fzmap)|*.fzmap";
            dialog.Title = "Load Map";
            if (!string.IsNullOrEmpty(tbTrackId.Text))
                dialog.FileName = $"{tbTrackId.Text}.fzmap";
            if (dialog.ShowDialog() == true)
            {
                svc.LoadMap(dialog.FileName);

                if (!string.IsNullOrEmpty(svc.MapPositions.TrackName))
                    tbTrackName.Text = svc.MapPositions.TrackName;

                tbTrackId.Text = svc.MapPositions.TrackId.ToString();
                tbXOffset.Text = svc.MapPositions.XOffset.ToString();
                tbZOffset.Text = svc.MapPositions.ZOffset.ToString();
                var scale = $"{(svc.MapPositions.MapScale * 100).ToString()}%";
                foreach (ComboBoxItem item in cbMapScale.Items)
                {
                    if (item.Content.ToString() == scale)
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
                miReduceMap.IsEnabled = true;
                ReplotPoints();
                result = true;
            }
            return result;
        }
        private void miReduceMap_Click(object sender, RoutedEventArgs e)
        {
            ReduceMap();
        }
        private void ReduceMap()
        {
            svc.ReduceMap();
            ReplotPoints();
        }
        private void tbnRotateMap_Toggle(object sender, RoutedEventArgs e)
        {
            if (tbnRotateMap.IsChecked == true)
            {
                svc.MapPositions.IsRotated = true;
                svc.Positions.IsRotated = true;
            }
            else
            {
                svc.MapPositions.IsRotated = false;
                svc.Positions.IsRotated = false;
            }
            ReplotPoints();
        }
        private void miLoadAndReduceMap_Click(object sender, RoutedEventArgs e)
        {
            if (LoadMap())
                ReduceMap();
        }
        #endregion

        #region Import / Export Data
        private void miExportData_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "FZTEL files (*.fztel)|*.fztel";
            dialog.Title = "Save Telemetry";
            dialog.FileName = $"{tbTrackId.Text}_telemetry.fztel";
            if (dialog.ShowDialog() == true)
            {
                MapSerializer.ExportPositionData(dialog.FileName, svc.Positions);
                MessageBox.Show("Telemetry Data Saved!");
            }
        }
        private void miImportData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "FZTEL files (*.fztel)|*.fztel";
            dialog.Title = "Load Telemtry";
            if (!string.IsNullOrEmpty(tbTrackId.Text))
                dialog.FileName = $"{tbTrackId.Text}.fztel";
            if (dialog.ShowDialog() == true)
            {
                svc.ImportTelemetry(dialog.FileName);
                tbXOffset.Text = svc.MapPositions.XOffset.ToString();
                tbZOffset.Text = svc.MapPositions.ZOffset.ToString();

                ReplotPoints();
                var laps = new List<int>();
                foreach (var position in svc.Positions.ExtendedPositions)
                {
                    if (!laps.Contains(position.LapNumber))
                        laps.Add(position.LapNumber);
                }
                var existingLaps = new List<int>();
                if (cbLapPoints.Items.Count > 2)
                {
                    for (var i = 2; i < cbLapPoints.Items.Count; i++)
                        existingLaps.Add((int)cbLapPoints.Items[i]);
                }
                foreach (var item in laps)
                {
                    if (!existingLaps.Contains(item))
                        cbLapPoints.Items.Add(item);
                }
                btnReplayData.IsEnabled = true;
                MessageBox.Show("Telemetry Loaded");
            }
        }
        #endregion

        #region Replay Data
        private void btnReplayData_Click(object sender, RoutedEventArgs e)
        {
            cMapPlot.Children.Clear();
            ReplotMap();
            replayTimer = new DispatcherTimer();
            replayTimer.Interval = TimeSpan.FromMilliseconds(16);
            replayTimer.Tick += ReplayData;

            // Start the timer
            replayTimer.Start();
            btnReplayData.IsEnabled = false;
            lReplayData.Content = "Replaying";
            mReplayMetrics.Visibility = Visibility.Visible;
            btnStopReplayData.IsEnabled = true;
            btnStopReplayData.Visibility = Visibility.Visible;
        }
        private void btnStopReplayData_Click(object sender, RoutedEventArgs e)
        {
            replayTimer.Stop();
            btnReplayData.IsEnabled = true;
            lReplayData.Content = "Replay";
            isReplaying = false;
            btnStopReplayData.IsEnabled = false;
            mReplayMetrics.Visibility = Visibility.Collapsed;
            btnStopReplayData.Visibility = Visibility.Collapsed;
        }
        private void ReplayData(object sender, EventArgs e)
        {
            if (CurrentOrdinal < (svc.Positions.ExtendedPositions.Count - 1))
            {
                var position = svc.Replay(svc.Positions.ExtendedPositions[CurrentOrdinal]);
                mReplayMetrics.ReceiveEvents(position);
                if (svc.CurrentLapEnded(svc.Positions.ExtendedPositions[CurrentOrdinal].LapNumber)) // Current Lap and Lap Has Ended...
                    cMapPlot.Children.Clear();

                if (svc.IsPlottedLap(svc.Positions.ExtendedPositions[CurrentOrdinal].LapNumber)) // if its all/current OR the lap number from the options... show it..
                {
                    AddCanvasPoint(position);
                    AddPlotLabels(CurrentOrdinal > 0, position, CurrentOrdinal > 0 ? svc.Positions.ExtendedPositions[CurrentOrdinal - 1] : null);
                }

                svc.CurrentLapNumber = svc.Positions.ExtendedPositions[CurrentOrdinal].LapNumber;
                CurrentOrdinal = CurrentOrdinal + 1;
                isReplaying = true;
            }
            else
            {
                replayTimer.Stop();
                btnReplayData.IsEnabled = true;
                lReplayData.Content = "Replay";
                isReplaying = false;
                mReplayMetrics.Visibility = Visibility.Collapsed;
                btnStopReplayData.IsEnabled = false;
                btnStopReplayData.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Handle Map Panning
        private void CoreMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousePressed = true;
            wasDragged = false;
            mousePressedInitialX = e.GetPosition(this).X;
            mousePressedInitialY = e.GetPosition(this).Y;
        }
        private void CoreMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mousePressed = false;
            if (wasDragged)
            {
                var deltaX = (int)mousePressedInitialX - (int)e.GetPosition(this).X;
                var deltaZ = (int)mousePressedInitialY - (int)e.GetPosition(this).Y;
                // once mouse has been lifted, adjust position based on end.
                svc.HandleMapPan(deltaX, deltaZ);

                tbXOffset.Text = svc.MapPositions.XOffset.ToString();
                tbZOffset.Text = svc.MapPositions.ZOffset.ToString();
                wasDragged = false;
                ReplotPoints();
            }
        }
        private void CoreMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressed)
                wasDragged = true;
        }
        #endregion

        #region Alter Map Visuals
        private void cbChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            svc.SetChartType((cbChartType.SelectedItem as ComboBoxItem)?.Content.ToString());
            ReplotPoints();
        }
        private void cbMapScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            svc.SetMapScale((cbMapScale.SelectedItem as ComboBoxItem)?.Content.ToString());
            ReplotPoints();
        }
        private void cbLapPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            svc.SetLapsToPlot((cbLapPoints.SelectedItem as ComboBoxItem)?.Content.ToString() ?? cbLapPoints.SelectedItem.ToString());
            ReplotPoints();
        }
        private void miIncludeLapTimes_Toggle(object sender, RoutedEventArgs e)
        {
            if (cMapPlot != null)
                cMapPlot.Children.Clear();
            ReplotPoints();
        }
        private void miShowGearChanges_Click(object sender, RoutedEventArgs e)
        {
            ReplotPoints();
        }
        private void AddLapsToDropDown(ref Telemetry payload)
        {
            if (svc.CurrentLapNumber != payload.Race.LapNumber)
            {
                var existingLaps = new List<int>();
                if (cbLapPoints.Items.Count > 2)
                {
                    for (var i = 2; i < cbLapPoints.Items.Count; i++)
                        existingLaps.Add((int)cbLapPoints.Items[i]);
                }
                if (!existingLaps.Contains(payload.Race.LapNumber))
                    cbLapPoints.Items.Add(payload.Race.LapNumber);

                AddLapTimePlotLabels(payload);
            }
        }
        #endregion
    }
}
