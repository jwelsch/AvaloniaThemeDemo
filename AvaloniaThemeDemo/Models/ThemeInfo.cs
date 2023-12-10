using Avalonia.Controls;

namespace AvaloniaThemeDemo.Models
{
    public class ThemeInfo
    {
        public int Index { get; }

        public string Name { get; }

        public IThemeVariantProvider ThemeVariantProvider { get; }

        public ThemeInfo(int index, string name, IThemeVariantProvider themeVariantProvider)
        {
            Index = index;
            Name = name;
            ThemeVariantProvider = themeVariantProvider;
        }
    }
}
