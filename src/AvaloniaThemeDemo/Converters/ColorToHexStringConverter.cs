using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AvaloniaThemeDemo.Converters
{
    public class ColorToHexStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null
                || value is not Color color)
            {
                return null;
            }

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
