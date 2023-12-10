using Avalonia.Media;

namespace AvaloniaThemeDemo.Models
{
    public class ThemeColor
    {
        public string Name { get; }

        public Color Color { get; }

        public ThemeColor(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
