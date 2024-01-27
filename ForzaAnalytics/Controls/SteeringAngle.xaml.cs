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
    /// Interaction logic for SteeringAngle.xaml
    /// </summary>
    public partial class SteeringAngle : UserControl
    {
        public SteeringAngle()
        {
            InitializeComponent();
        }
        public void Update(double value)
        {
            var angle = SteeringAngleToDegrees(value);
            if (angle > 90)
                angle = -90;
            lValue.Content = $"{Math.Round((angle / 90) * 100, 0)} %";
            lAngle.Angle = angle;
        }

        private double SteeringAngleToDegrees(double value)
        {
            double result = 0;
            if (value > 129) // 255->129 = Left Turn. 129 = Full Lock Left
            {
                result = Map(value, 255, 129, 0, -90);   
            }
            else // 1->127 = Right Turn. 127 = Full Lock Right
            {
                result = Map(value, 0, 127, 0, 90);
            }
            return result;
        }

        private static double Map(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
