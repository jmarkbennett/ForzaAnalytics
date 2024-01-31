using System.Diagnostics.Eventing.Reader;
using System.Windows.Media;

namespace ForzaAnalytics.Helpers
{
    internal static class ColourHelper
    {
        internal static SolidColorBrush GetColourFromString(string colorStr)
        {
            if (colorStr.Length == 6)
            {
                byte r = byte.Parse(colorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(colorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(colorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new SolidColorBrush(Color.FromRgb(r, g, b));
            }
            else
            {
                byte a = byte.Parse(colorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte r = byte.Parse(colorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(colorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(colorStr.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
        }
    }
}
