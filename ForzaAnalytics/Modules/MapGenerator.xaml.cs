using ForzaAnalytics.Models.Helpers;
using ForzaAnalytics.Services.Helpers;
using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.Services.Service;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for MapGenerator.xaml
    /// </summary>
    public partial class MapGenerator : UserControl
    {
        private MapGeneratorService svc;
        private bool mousePressed = false;
        private bool wasDragged = false;
        private double mousePressedInitialX = 0;
        private double mousePressedInitialY = 0;
        public MapGenerator()
        {
            svc = new MapGeneratorService();

            InitializeComponent();
            cMapPlot.Height = svc.InitialCanvasSize;
            cMapPlot.Width = svc.InitialCanvasSize;
        }
        public void ReceiveEvents(Telemetry payload)
        {
            tbTrackId.Text = payload.Race.TrackIdentifier.ToString();
            if (svc.IsTracking)
            {
                var position = svc.Update(payload);
                AddCanvasPoint(position);
                btnCommit.IsEnabled = true;
            }
            if (svc.HasLapChanged)
            {
                tglTrackData.IsChecked = false;
                svc.HasLapChanged = false;
            }
        }
        private void AddCanvasPoint(Models.Core.PositionalData position)
        {
            Ellipse dot = new Ellipse
            {
                Width = 2,
                Height = 2,
                Fill = Brushes.DarkGray
            };

            Point canvasPoint = new Point(
                svc.Positions.GetAdjustedXCoordinate(position.X, position.Z),
                svc.Positions.GetAdjustedZCoordinate(position.X, position.Z));

            Canvas.SetLeft(dot, canvasPoint.X);
            Canvas.SetTop(dot, canvasPoint.Y);
            cMapPlot.Children.Add(dot);
        }
        public void ResetEvents()
        {
            svc.ResetService();
            if (cMapPlot.Children != null)
                cMapPlot.Children.Clear();
            btnCommit.IsEnabled = false;
            btnSave.IsEnabled = false;
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetEvents();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (svc.AllPositions.Positions.Count > 0)
            {
                svc.UpdateTrackName(tbTrackName.Text);

                if (svc.Positions.Positions.Count > 0)
                    MessageBox.Show("You Have Uncommitted Positions. please either commit these or 'Recent Latest'");
                else
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Filter = "FZMAP files (*.fzmap)|*.fzmap";
                    dialog.Title = "Save Map";
                    dialog.FileName = $"{svc.AllPositions.TrackId}.fzmap";
                    if (dialog.ShowDialog() == true)
                    {
                        MapSerializer.PersistMap(dialog.FileName, svc.AllPositions);
                        MessageBox.Show("Map Saved!");
                    }
                }
            }
            else
            {
                if (svc.Positions.Positions.Count > 0)
                    MessageBox.Show("No Positions have been commited, but you have some uncommited. Please commit these first");
                else
                    MessageBox.Show("No Positions have been commited to save!");
            }
        }
        private void tglTrackData_Checked(object sender, RoutedEventArgs e)
        {
            svc.IsTracking = true;
            tglTrackData.Content = "Stop Tracking";
            eIsListening.Fill = new SolidColorBrush(Colors.YellowGreen);
        }
        private void tglTrackData_Unchecked(object sender, RoutedEventArgs e)
        {
            svc.IsTracking = false;
            tglTrackData.Content = "Start Tracking";
            eIsListening.Fill = new SolidColorBrush(Colors.Red);
        }
        private void btnApplyOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                svc.Positions.XOffset = int.Parse(tbXOffset.Text);
                svc.AllPositions.XOffset = int.Parse(tbXOffset.Text);
            }
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                svc.Positions.ZOffset = int.Parse(tbZOffset.Text);
                svc.AllPositions.ZOffset = int.Parse(tbZOffset.Text);
            }
            ReplotPoints();
        }
        private void ReduceMap()
        {
            if (tbReducePrecision.Text != null)
            {
                svc.ReduceMap(int.Parse(tbReducePrecision.Text));
                ReplotPoints();
            }
        }
        private void ReplotPoints()
        {
            if (cMapPlot != null)
            {
                cMapPlot.Children.Clear();
                foreach (var position in svc.AllPositions.Positions)
                    AddCanvasPoint(position);
                foreach (var position in svc.Positions.Positions)
                    AddCanvasPoint(position);
            }
        }
        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            var result = svc.CommitPositions();
            if (result > 0)
            {
                MessageBox.Show($"Committed {result} Row(s)");
                ReplotPoints();
                btnCommit.IsEnabled = false;
                btnSave.IsEnabled = true;
            }
            else
                MessageBox.Show("No Positions have been tracked", "No Positions Tracked");
        }
        private void btnResetLatest_Click(object sender, RoutedEventArgs e)
        {
            svc.Positions.Positions.Clear();
            ReplotPoints();
        }
        private void btnReduceMap_Click(object sender, RoutedEventArgs e)
        {
            ReduceMap();
        }
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

                tbXOffset.Text = svc.Positions.XOffset.ToString();
                tbZOffset.Text = svc.Positions.ZOffset.ToString();
                wasDragged = false;
                ReplotPoints();
            }
        }
        private void CoreMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressed)
                wasDragged = true;
        }
        private void cbMapScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            svc.SetMapScale(((ComboBoxItem)cbMapScale.SelectedItem)?.Content.ToString() ?? string.Empty);
            ReplotPoints();
        }
        private void btnXOffsetDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                tbXOffset.Text = (int.Parse(tbXOffset.Text) - 10).ToString();
                svc.Positions.XOffset = int.Parse(tbXOffset.Text);
                svc.AllPositions.XOffset = int.Parse(tbXOffset.Text);
                ReplotPoints();
            }
        }
        private void btnXOffsetUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                tbXOffset.Text = (int.Parse(tbXOffset.Text) + 10).ToString();
                svc.Positions.XOffset = int.Parse(tbXOffset.Text);
                svc.AllPositions.XOffset = int.Parse(tbXOffset.Text);
                ReplotPoints();
            }
        }
        private void btnZOffsetDown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                tbZOffset.Text = (int.Parse(tbZOffset.Text) - 10).ToString();
                svc.Positions.ZOffset = int.Parse(tbZOffset.Text);
                svc.AllPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void btnZOffsetUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                tbZOffset.Text = (int.Parse(tbZOffset.Text) + 10).ToString();
                svc.Positions.ZOffset = int.Parse(tbZOffset.Text);
                svc.AllPositions.ZOffset = int.Parse(tbZOffset.Text);
                ReplotPoints();
            }
        }
        private void btnSuggestOffset_Click(object sender, RoutedEventArgs e)
        {
            svc.Positions.XOffset = (int)svc.GetSuggestedXOffset();
            svc.AllPositions.XOffset = (int)svc.GetSuggestedXOffset();
            svc.Positions.ZOffset = (int)svc.GetSuggestedZOffset();
            svc.AllPositions.ZOffset = (int)svc.GetSuggestedZOffset();
            tbXOffset.Text = svc.AllPositions.XOffset.ToString();
            tbZOffset.Text = svc.AllPositions.ZOffset.ToString();
            ReplotPoints();
        }
        private void tbnRotateMap_Toggle(object sender, RoutedEventArgs e)
        {
            if (tbnRotateMap.IsChecked == true)
            {
                svc.AllPositions.IsRotated = true;
                svc.Positions.IsRotated = true;
            }
            else
            {
                svc.AllPositions.IsRotated = false;
                svc.Positions.IsRotated = false;
            }
            ReplotPoints();
        }
    }
}