using System.Windows.Media;

namespace ForzaAnalytics.Helpers
{
    public static class ColourHelper
    {
        public static SolidColorBrush GetColourFromString(string colorStr)
        {
            byte r = byte.Parse(colorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(colorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(colorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
        }
    }
}
