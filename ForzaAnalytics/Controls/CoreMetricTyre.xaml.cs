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
            pbFl.Value = payload.Tire.TyreWearRemainingFrontLeft;
            pbFl.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingFrontLeft);
            lflValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tire.TyreWearRemainingFrontLeft);
            lflValue.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingFrontLeft);

            pbFr.Value = payload.Tire.TyreWearRemainingFrontRight;
            pbFr.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingFrontRight);
            lfrValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tire.TyreWearRemainingFrontRight);
            lfrValue.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingFrontRight);

            pbRl.Value = payload.Tire.TyreWearRemainingRearLeft;
            pbRl.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingRearLeft);
            lrlValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tire.TyreWearRemainingRearLeft);
            lrlValue.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingRearLeft);

            pbRr.Value = payload.Tire.TyreWearRemainingRearRight;
            pbRr.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingRearRight);
            lrrValue.Content = Models.Formatters.Formatting.FormattedPercentage(payload.Tire.TyreWearRemainingRearRight);
            lrrValue.Foreground = GetTyreWearColour(payload.Tire.TyreWearRemainingRearRight);
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
