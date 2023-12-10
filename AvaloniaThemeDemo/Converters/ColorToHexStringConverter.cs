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
            return value is Color color ? $"#{color.R:X2}{color.G:X2}{color.B:X2}" : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
