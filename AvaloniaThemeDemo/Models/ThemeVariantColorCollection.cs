using System.Collections;
using System.Collections.Generic;

namespace AvaloniaThemeDemo.Models
{
    public interface IThemeVariantColorCollection : IEnumerable<ThemeColor>
    {
        /// <summary>
        /// The name of the theme (e.g., "Simple", "Fluent", etc.).
        /// </summary>
        string? ThemeName { get; }

        /// <summary>
        /// The name of the theme variant (e.g., "Default", "Dark", "Light", etc.).
        /// </summary>
        string? VariantName { get; }

        string? Name { get; }

        void Add(ThemeColor themeColor);

        bool Contains(ThemeColor themeColor);
    }

    public class ThemeVariantColorCollection : IThemeVariantColorCollection
    {
        private readonly List<ThemeColor> _themeColors = new();

        /// <summary>
        /// The name of the theme (e.g., "Simple", "Fluent", etc.).
        /// </summary>
        public string? ThemeName { get; }

        /// <summary>
        /// The name of the theme variant (e.g., "Default", "Dark", "Light", etc.).
        /// </summary>
        public string? VariantName { get; }

        public string? Name => ThemeName == null || VariantName == null ? "" : $"{ThemeName} {VariantName}";

        public ThemeVariantColorCollection(string? themeName, string? variantName)
        {
            ThemeName = themeName;
            VariantName = variantName;
        }

        public void Add(ThemeColor themeColor)
        {
            _themeColors.Add(themeColor);
        }

        public bool Contains(ThemeColor themeColor)
        {
            for (var i = 0; i < _themeColors.Count; i++)
            {
                if (themeColor.Key == _themeColors[i].Key)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<ThemeColor> GetEnumerator()
        {
            return _themeColors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
