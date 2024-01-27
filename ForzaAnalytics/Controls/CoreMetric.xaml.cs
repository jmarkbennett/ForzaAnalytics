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
    /// Interaction logic for CoreMetric.xaml
    /// </summary>
    public partial class CoreMetric : UserControl
    {
        public static readonly DependencyProperty MetricTitleProperty =
        DependencyProperty.Register("MetricTitle", typeof(string), typeof(CoreMetric));

        public string MetricTitle
        {
            get { return (string)GetValue(MetricTitleProperty); }
            set { SetValue(MetricTitleProperty, value); }
        }

        public CoreMetric()
        {
            InitializeComponent();
        }

        public void Update(string value)
        {
            lMetricValue.Content = value;
        }
    }
}
