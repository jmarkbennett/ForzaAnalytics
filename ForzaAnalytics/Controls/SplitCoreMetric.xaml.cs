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
    /// Interaction logic for SplitCoreMetric.xaml
    /// </summary>
    public partial class SplitCoreMetric : UserControl
    {
        public static readonly DependencyProperty PrimaryMetricTitleProperty =
        DependencyProperty.Register("PrimaryMetricTitle", typeof(string), typeof(CoreMetric));

        public static readonly DependencyProperty SecondaryMetricTitleProperty =
        DependencyProperty.Register("SecondaryMetricTitle", typeof(string), typeof(CoreMetric));

        public static readonly DependencyProperty SubTitleProperty =
        DependencyProperty.Register("SubTitle", typeof(string), typeof(CoreMetric));


        public string PrimaryMetricTitle
        {
            get { return (string)GetValue(PrimaryMetricTitleProperty); }
            set { SetValue(PrimaryMetricTitleProperty, value); }
        }

        public string SecondaryMetricTitle
        {
            get { return (string)GetValue(SecondaryMetricTitleProperty); }
            set { SetValue(SecondaryMetricTitleProperty, value); }
        }

        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        public SplitCoreMetric()
        {
            InitializeComponent();
        }

        public void Update(string primaryValue, string secondaryValue, string subTitle)
        {
            lPrimaryMetricValue.Content = primaryValue;
            lSecondaryMetricValue.Content = secondaryValue;
            lSubTitle.Content = subTitle;
        }
    }
}
