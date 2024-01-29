using ForzaAnalytics.Models.Helpers;
using ForzaAnalytics.Services.Serializers;
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
        private bool mousePressed = false;
        private bool wasDragged = false;
        private double mousePressedInitialX = 0;
        private double mousePressedInitialY = 0;


        private int lapNumber;
        private bool isTracking = false;
        private Models.Core.GroupedPositionalData positions; // Current tracking..
        private Models.Core.GroupedPositionalData allPositions; // Full tracking...

        public float? minX { get; set; }
        public float? maxX{ get; set; }
        public float? minZ{ get; set; }
        public float? maxZ{ get; set; }

        public MapGenerator()
        {
            lapNumber = 0;
            positions = new Models.Core.GroupedPositionalData();
            allPositions = new Models.Core.GroupedPositionalData();
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {
            tbTrackId.Text = payload.Race.TrackIdentifier.ToString();
            positions.TrackId = payload.Race.TrackIdentifier;
            allPositions.TrackId = payload.Race.TrackIdentifier;
            if (isTracking)
            {
                if (maxX == null)
                {
                    maxX = payload.Position.PositionX;
                    tbMaxX.Text = maxX.ToString();
                }
            else
            {
                if (maxX < payload.Position.PositionX)
                {
                    maxX = payload.Position.PositionX;
                    tbMaxX.Text = maxX.ToString();
                }
            }
            if (maxZ == null)
            {
                maxZ = payload.Position.PositionZ;
                tbMaxZ.Text = maxZ.ToString();
            }
            else
            {
                if (maxZ < payload.Position.PositionZ)
                {
                    maxZ = payload.Position.PositionZ;
                    tbMaxZ.Text = maxZ.ToString();
                }
            }

            if (minX == null)
            {
                minX = payload.Position.PositionX;
                tbMinX.Text = minX.ToString();
            }
            else
            {
                if (minX > payload.Position.PositionX)
                {
                    minX = payload.Position.PositionX;
                    tbMinX.Text = minX.ToString();
                }
            }
                if (minZ == null)
                {
                    minZ = payload.Position.PositionZ;
                    tbMinZ.Text = minZ.ToString();
                }
                else
                {
                    if (minZ > payload.Position.PositionZ)
                    {
                        minZ = payload.Position.PositionZ;
                        tbMinZ.Text = minZ.ToString();
                    }
                }

                if (payload.Race.LapNumber == lapNumber)
                {
                    positions.Positions.Add(new Models.Core.PositionalData(payload.Position.PositionX, payload.Position.PositionY, payload.Position.PositionZ));
                    Ellipse dot = new Ellipse
                    {
                        Width = 2,
                        Height = 2,
                        Fill = Brushes.DarkGray
                    };

                    Point canvasPoint = new Point(
                        payload.Position.PositionX + (positions.XOffset),
                        payload.Position.PositionZ + (positions.ZOffset));
                    Canvas.SetLeft(dot, canvasPoint.X);
                    Canvas.SetTop(dot, canvasPoint.Y);
                    cMapPlot.Children.Add(dot);
                }
                else
                {
                    tglTrackData.IsChecked = false;
                    isTracking = false;
                    lapNumber = payload.Race.LapNumber;
                }
            }
        }
        public void ResetEvents()
        {
            lapNumber = 0;

            positions = new Models.Core.GroupedPositionalData();
            allPositions = new Models.Core.GroupedPositionalData();

            minX = 0;
            maxX = 0;
            minZ = 0;
            maxZ = 0;
            if (cMapPlot.Children != null)
                cMapPlot.Children.Clear();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            allPositions.ResetPositions();
            positions.ResetPositions();
            ReplotPoints();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (allPositions.Positions.Count > 0)
            {
                if (!string.IsNullOrEmpty(tbTrackName.Text))
                {
                    allPositions.TrackName = tbTrackName.Text;
                    positions.TrackName = tbTrackName.Text;
                }

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "FZMAP files (*.fzmap)|*.fzmap";
                dialog.Title = "Save Map";
                dialog.FileName = $"{allPositions.TrackId}.fzmap";
                if (dialog.ShowDialog() == true)
                {
                    MapSerializer.PersistMap(dialog.FileName, allPositions);
                }
            }
            else
            {
                if(positions.Positions.Count > 0)
                    MessageBox.Show("No Positions have been commited, but you have some uncommited. Please commit these first");
                else
                    MessageBox.Show("No Positions have been commited to save!");
            }
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

        private void btnApplyOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbXOffset.Text))
            {
                positions.XOffset = int.Parse(tbXOffset.Text);
                allPositions.XOffset = int.Parse(tbXOffset.Text);
            }
            if (!string.IsNullOrEmpty(tbZOffset.Text))
            {
                positions.ZOffset = int.Parse(tbZOffset.Text);
                allPositions.ZOffset = int.Parse(tbZOffset.Text);
            }
            ReplotPoints();
        }

        private void ReduceMap()
        {
            var newPositions = new List<Models.Core.PositionalData>();
            if (tbReducePrecision.Text != null)
            {
                foreach (var row in allPositions.Positions)
                    newPositions.Add(
                        new Models.Core.PositionalData(
                            (float)Math.Round(row.X, int.Parse(tbReducePrecision.Text)),
                            (float)Math.Round(row.Y, int.Parse(tbReducePrecision.Text)),
                            (float)Math.Round(row.Z, int.Parse(tbReducePrecision.Text))
                            )
                    );
                allPositions.Positions = newPositions.Distinct(new PositionalDataComparer()).ToList();
                newPositions = new List<Models.Core.PositionalData>();
                foreach (var row in positions.Positions)
                    newPositions.Add(
                        new Models.Core.PositionalData(
                            (float)Math.Round(row.X, int.Parse(tbReducePrecision.Text)), 
                            (float)Math.Round(row.Y, int.Parse(tbReducePrecision.Text)), 
                            (float)Math.Round(row.Z, int.Parse(tbReducePrecision.Text))
                            )
                    );
                positions.Positions = newPositions.Distinct(new PositionalDataComparer()).ToList();
                ReplotPoints();
            }
        }

        private void ReplotPoints()
        {
            cMapPlot.Children.Clear();
            foreach (var position in allPositions.GetAdjustedPositions())
            {
                Ellipse dot = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.DarkGray
                };

                Point canvasPoint = new Point(position.X, position.Z);
                Canvas.SetLeft(dot, canvasPoint.X);
                Canvas.SetTop(dot, canvasPoint.Y);
                cMapPlot.Children.Add(dot);
            }
            foreach (var position in positions.GetAdjustedPositions())
            {
                Ellipse dot = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = Brushes.DarkGray
                };

                Point canvasPoint = new Point(position.X, position.Z);
                Canvas.SetLeft(dot, canvasPoint.X);
                Canvas.SetTop(dot, canvasPoint.Y);
                cMapPlot.Children.Add(dot);
            }
            ResizeCanvas();
        }

        private void ResizeCanvas()
        {
            var tmp = allPositions.GetAdjustedPositions();
            if (tmp != null && tmp.Count < 0)
            {
                float minX = tmp.Min(x => x.X);
                float minZ = tmp.Min(x => x.Z);
                float maxX = tmp.Max(x => x.X);
                float maxZ = tmp.Max(x => x.Z);
                cMapPlot.Height = maxZ - minZ;
                cMapPlot.Width = maxX - minX;
            }
            tmp = positions.GetAdjustedPositions();
            if (tmp != null && tmp.Count < 0)
            {
                float minX = tmp.Min(x => x.X);
                float minZ = tmp.Min(x => x.Z);
                float maxX = tmp.Max(x => x.X);
                float maxZ = tmp.Max(x => x.Z);
                cMapPlot.Height = maxZ - minZ;
                cMapPlot.Width = maxX - minX;
            }
        }

        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            if (positions.Positions.Count > 0)
            {
                allPositions.Positions.AddRange(positions.Positions);
            }
            else { MessageBox.Show("No Positions have been tracked", "No Positions Tracked"); }
        }

        private void btnResetLatest_Click(object sender, RoutedEventArgs e)
        {
            positions.Positions.Clear();
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
                positions.XOffset = positions.XOffset - deltaX;
                positions.ZOffset = positions.ZOffset + deltaZ;
                allPositions.XOffset = positions.XOffset;
                allPositions.ZOffset = positions.ZOffset;

                tbXOffset.Text = positions.XOffset.ToString();
                tbZOffset.Text = positions.ZOffset.ToString();
                wasDragged = false;
                ReplotPoints();
            }

        }

        private void CoreMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressed)
                wasDragged = true;
        }
    }
}
