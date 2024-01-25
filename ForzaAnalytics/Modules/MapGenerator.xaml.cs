using ForzaAnalytics.Services.Serializers;
using ForzaAnalytics.UdpReader.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ForzaAnalytics.Modules
{
    /// <summary>
    /// Interaction logic for MapGenerator.xaml
    /// </summary>
    public partial class MapGenerator : UserControl
    {
        private bool isTracking = false;
        private Models.Core.GroupedPositionalData positions;
        public MapGenerator()
        {
            positions = new Models.Core.GroupedPositionalData();
            InitializeComponent();
        }
        public void ReceiveEvents(Telemetry payload)
        {
            tbTrackId.Text = payload.Race.TrackIdentifier.ToString();
            positions.TrackId = payload.Race.TrackIdentifier;
            if (isTracking)
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbTrackName.Text))
                positions.TrackName = tbTrackName.Text;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Save Map";
            dialog.FileName = $"{positions.TrackId}.json";
            if (dialog.ShowDialog() == true)
            {
                MapSerializer.PersistMap(dialog.FileName, positions);
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
                positions.XOffset = int.Parse(tbXOffset.Text);
            if(!string.IsNullOrEmpty(tbZOffset.Text))
                positions.ZOffset = int.Parse(tbZOffset.Text);

            ReplotPoints();
        }

        private void ReplotPoints()
        {
            cMapPlot.Children.Clear();
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
            var tmp = positions.GetAdjustedPositions();
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
    }
}
