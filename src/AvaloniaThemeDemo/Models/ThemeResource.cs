using Avalonia.Media;

namespace AvaloniaThemeDemo.Models
{
    public interface IThemeResource<T>
    {
        string? ThemeName { get; }

        string? VariantName { get; }

        string Key { get; }

        T Resource { get; }
    }

    public class ThemeResource<T> : IThemeResource<T>
    {
        public string? ThemeName { get; }

        public string? VariantName { get; }

        public string Key { get; }

        public T Resource { get; }

        public ThemeResource(string? themeName, string? variantName, string key, T resource)
        {
            ThemeName = themeName;
            VariantName = variantName;
            Key = key;
            Resource = resource;
        }
    }

    public interface IThemeColor : IThemeResource<Color>
    {
        Color Color { get; }
    }

    public class ThemeColor : ThemeResource<Color>, IThemeColor
    {
        public Color Color { get; }

        public ThemeColor(string? themeName, string? variantName, string key, Color color)
            : base(themeName, variantName, key, color)
        {
            Color = color;
        }
    }
}
