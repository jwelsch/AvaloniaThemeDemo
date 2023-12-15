namespace AvaloniaThemeDemo.Models
{
    public interface IThemeVariantColors
    {
        string? ThemeName { get; }

        string? ThemeVariantName { get; }

        string? Name { get; }

        ThemeVariantColorCollection Colors { get; }
    }

    public class ThemeVariantColors : IThemeVariantColors
    {
        public string? ThemeName { get; }

        public string? ThemeVariantName { get; }

        public string? Name => ThemeName == null || ThemeVariantName == null ? "" : $"{ThemeName} {ThemeVariantName}";

        public ThemeVariantColorCollection Colors { get; }

        public ThemeVariantColors(string? themeName, string? themeVariantName, ThemeVariantColorCollection colors)
        {
            ThemeName = themeName;
            ThemeVariantName = themeVariantName;
            Colors = colors;
        }
    }
}
