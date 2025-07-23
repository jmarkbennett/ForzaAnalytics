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
    public partial class PodiumStatMetric : UserControl
    {   
        public PodiumStatMetric()
        {
            InitializeComponent();
        }
        public void Update(string first, string second, string third)
        {
            l1stValue.Content = first;
            l2ndValue.Content = second;
            l3rdValue.Content = third;
        }
    }
}
