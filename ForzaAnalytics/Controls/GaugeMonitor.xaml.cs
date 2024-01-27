using System;
using System.Collections.Generic;
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
    /// Interaction logic for GaugeMonitor.xaml
    /// </summary>
    public partial class GaugeMonitor : UserControl
    {
        public static readonly DependencyProperty GaugeTitleProperty =
        DependencyProperty.Register("GaugeTitle", typeof(string), typeof(GaugeMonitor));

        public static readonly DependencyProperty GaugeColourProperty =
        DependencyProperty.Register("GaugeColour", typeof(Brush), typeof(GaugeMonitor));

        public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(double), typeof(GaugeMonitor));

        public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(double), typeof(GaugeMonitor));

        public static readonly DependencyProperty AltColourTitleProperty =
        DependencyProperty.Register("AltColourTitle", typeof(Brush), typeof(GaugeMonitor));

        public static readonly DependencyProperty AlertGaugeColourProperty =
        DependencyProperty.Register("AlertGaugeColour", typeof(Brush), typeof(GaugeMonitor));

        public string GaugeTitle
        {
            get { return (string)GetValue(GaugeTitleProperty); }
            set { SetValue(GaugeTitleProperty, value); }
        }
        public Brush GaugeColour
        {
            get { return (Brush)GetValue(GaugeColourProperty); }
            set { SetValue(GaugeColourProperty, value); }
        }

        public Brush AlertGaugeColour
        {
            get { return (Brush)GetValue(AlertGaugeColourProperty); }
            set { SetValue(AlertGaugeColourProperty, value); }
        }

        public Brush AltColourTitle
        {
            get { return (Brush)GetValue(AltColourTitleProperty); }
            set { SetValue(AltColourTitleProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public GaugeMonitor()
        {
            InitializeComponent();
            if (this.AltColourTitle != null)
            {
                lValue.Foreground = AltColourTitle;
            }
        }

        public void Update(int value, string displayValue)
        {
            lValue.Content = displayValue;
            pbValue.Value = value;
            HandleColour();
        }
        public void Update(double value, string displayValue)
        {
            lValue.Content = displayValue;
            pbValue.Value = value;
            HandleColour();
        }
        public void Update(float value, string displayValue)
        {
            lValue.Content = displayValue;
            pbValue.Value = value;
            HandleColour();
        }

        private void HandleColour()
        {
            if (AlertGaugeColour != null)
            {
                if (pbValue.Value > (pbValue.Maximum * .85))
                {
                    pbValue.Foreground = AlertGaugeColour;
                    lValue.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    pbValue.Foreground = GaugeColour;
                    lValue.Foreground = AltColourTitle ?? new SolidColorBrush(Colors.White);
                }
            } 
        }
    }
}