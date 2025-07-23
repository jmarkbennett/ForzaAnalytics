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
    public partial class GenericStatMetric : UserControl
    {
        public static readonly DependencyProperty StatTitleProperty =
        DependencyProperty.Register("StatTitle", typeof(string), typeof(GenericStatMetric));

        public static readonly DependencyProperty StatIconProperty =
            DependencyProperty.Register("StatIcon", typeof(string), typeof(GenericStatMetric));
        public string StatTitle
        {
            get { return (string)GetValue(StatTitleProperty); }
            set { SetValue(StatTitleProperty, value); }
        }

        public string StatIcon
        {
            get { return (string)GetValue(StatIconProperty); }
            set { SetValue(StatIconProperty, value); }
        }
        public GenericStatMetric()
        {
            InitializeComponent();
       
        }
        public void Update(string value)
        {
            lMetricValue.Content = value;
        }
    }
}
