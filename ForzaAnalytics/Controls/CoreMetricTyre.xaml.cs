using ForzaAnalytics.UdpReader.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForzaAnalytics.Controls
{
    /// <summary>
    /// Interaction logic for CoreMetric.xaml
    /// </summary>
    public partial class CoreMetricTyre : UserControl
    {
        public static readonly DependencyProperty MetricTitleProperty =
        DependencyProperty.Register("MetricTitle", typeof(string), typeof(CoreMetricTyre));
        public string MetricTitle
        {
            get { return (string)GetValue(MetricTitleProperty); }
            set { SetValue(MetricTitleProperty, value); }
        }
        public CoreMetricTyre()
        {
            InitializeComponent();
        }
        public void Update(ref Telemetry payload)
        {
            pbFl.Value = payload.Tyre.TyreWearRemainingFrontLeft;
            pbFl.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingFrontLeft);
            lflValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tyre.TyreWearRemainingFrontLeft);
            lflValue.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingFrontLeft);

            pbFr.Value = payload.Tyre.TyreWearRemainingFrontRight;
            pbFr.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingFrontRight);
            lfrValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tyre.TyreWearRemainingFrontRight);
            lfrValue.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingFrontRight);

            pbRl.Value = payload.Tyre.TyreWearRemainingRearLeft;
            pbRl.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingRearLeft);
            lrlValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tyre.TyreWearRemainingRearLeft);
            lrlValue.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingRearLeft);

            pbRr.Value = payload.Tyre.TyreWearRemainingRearRight;
            pbRr.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingRearRight);
            lrrValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tyre.TyreWearRemainingRearRight);
            lrrValue.Foreground = GetTyreWearColour(payload.Tyre.TyreWearRemainingRearRight);
        }

        public static System.Windows.Media.Brush GetTyreWearColour(float wear)
        {
            if (wear < 0.2)
                return new SolidColorBrush(Colors.DarkRed);
            if (wear < 0.3)
                return new SolidColorBrush(Colors.Red);
            if (wear < 0.4)
                return new SolidColorBrush(Colors.OrangeRed);
            if (wear < 0.5)
                return new SolidColorBrush(Colors.Yellow);
            if (wear < 0.6)
                return new SolidColorBrush(Colors.LightYellow);
            else 
                return new SolidColorBrush(Colors.White);
        }
    }
}
