using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaThemeDemo.Models;
using AvaloniaThemeDemo.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaThemeDemo.Services
{
    public interface IThemeResourceProvider
    {
        List<ThemeVariantColorCollection>? GetAllThemeVariantColors();

        ThemeVariantColorCollection? GetThemeVariantColors(string themeName, string themeVariantName);
    }

    public class ThemeResourceProvider : IThemeResourceProvider
    {
        private List<ThemeVariantColorCollection>? _themes;

        public List<ThemeVariantColorCollection>? GetAllThemeVariantColors()
        {
            _themes ??= FindThemes();

            return _themes;
        }

        public ThemeVariantColorCollection? GetThemeVariantColors(string themeName, string themeVariantName)
        {
            _themes ??= FindThemes();

            var themeVariantColorCollection = _themes.FirstOrDefault(i => string.Equals(i.ThemeName, themeName, StringComparison.OrdinalIgnoreCase)
                                                                     && string.Equals(i.VariantName, themeVariantName, StringComparison.OrdinalIgnoreCase));

            return themeVariantColorCollection;
        }

        private static List<ThemeVariantColorCollection> FindThemes()
        {
            var themes = new List<ThemeVariantColorCollection>();

            if (Application.Current?.Styles == null)
            {
                System.Diagnostics.Trace.WriteLine($"Application does not have any styles.");
                return themes;
            }

            var colorList = new List<ThemeColor>();

            foreach (var style in Application.Current.Styles)
            {
                if (style is Styles theStyle)
                {
                    var styleType = style.GetType();

                    var themeName = styleType.Name.Replace("Theme", "");

                    foreach (var resource in theStyle.Resources)
                    {
                        var resourceColors = FindResourceDictionaryColors(themeName, null, theStyle.Resources);

                        if (resourceColors != null && resourceColors.Count > 0)
                        {
                            colorList.AddRange(resourceColors);
                        }
                    }
                }
            }

            themes = MergeThemeColors(colorList);

            return themes;
        }

        private static IList<ThemeColor>? FindResourceDictionaryColors(string? themeName, string? themeVariantName, IResourceDictionary resourceDictionary)
        {
            var colorList = new List<ThemeColor>();

            foreach (var kvp in resourceDictionary)
            {
                if (kvp.Key == null)
                {
                    continue;
                }

                var value = resourceDictionary[kvp.Key];

                if (value is Color color)
                {
                    colorList.Add(new ThemeColor(themeName, themeVariantName, kvp.Key?.ToString() ?? "", color));
                }
                else if (value is SolidColorBrush solidBrush)
                {
                    colorList.Add(new ThemeColor(themeName, themeVariantName, kvp.Key?.ToString() ?? "", solidBrush.Color));
                }
            }

            var mergedColors = FindMergedDictionaryColors(themeName, themeVariantName, resourceDictionary.MergedDictionaries);

            if (mergedColors != null && mergedColors.Count > 0)
            {
                colorList.AddRange(mergedColors);
            }

            var themeColors = FindThemeDictionaryColors(themeName, themeVariantName, resourceDictionary.ThemeDictionaries);

            if (themeColors != null && themeColors.Count > 0)
            {
                colorList.AddRange(themeColors);
            }

            return colorList;
        }

        private static IList<ThemeColor>? FindMergedDictionaryColors(string? themeName, string? themeVariantName, IList<IResourceProvider> mergedDictionary)
        {
            var colorList = new List<ThemeColor>();

            foreach (var kvp in mergedDictionary)
            {
                var type = kvp.GetType();

                // TODO: System accent colors

                if (kvp is IResourceDictionary resourceDictionary)
                {
                    var resourceColors = FindResourceDictionaryColors(themeName, themeVariantName, resourceDictionary);

                    if (resourceColors != null && resourceColors.Count > 0)
                    {
                        colorList.AddRange(resourceColors);
                    }
                }
                else if (type.FullName == "Avalonia.Themes.Fluent.Accents.SystemAccentColors")
                {
                    var systemAccentColorsAccessor = new SystemAccentColorsAccessor(kvp);
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColor), systemAccentColorsAccessor!.SystemAccentColor!.Value));
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColorDark1), systemAccentColorsAccessor!.SystemAccentColorDark1!.Value));
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColorDark2), systemAccentColorsAccessor!.SystemAccentColorDark2!.Value));
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColorDark3), systemAccentColorsAccessor!.SystemAccentColorDark3!.Value));
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColorLight1), systemAccentColorsAccessor!.SystemAccentColorLight1!.Value));
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColorLight2), systemAccentColorsAccessor!.SystemAccentColorLight2!.Value));
                    colorList.Add(new ThemeColor(themeName, themeVariantName, nameof(systemAccentColorsAccessor.SystemAccentColorLight3), systemAccentColorsAccessor!.SystemAccentColorLight3!.Value));
                }
            }

            return colorList;
        }

        private static IList<ThemeColor>? FindThemeDictionaryColors(string? themeName, string? themeVariantName, IDictionary<ThemeVariant, IThemeVariantProvider> themeDictionary)
        {
            var colorList = new List<ThemeColor>();

            foreach (var kvp in themeDictionary)
            {
                var value = themeDictionary[kvp.Key];

                if (value is IResourceDictionary resourceDictionary)
                {
                    var resourceColors = FindResourceDictionaryColors(themeName, kvp.Key.ToString(), resourceDictionary);

                    if (resourceColors != null)
                    {
                        colorList.AddRange(resourceColors);
                    }
                }
            }

            return colorList;
        }

        private static List<ThemeVariantColorCollection> MergeThemeColors(List<ThemeColor> themeColors)
        {
            var mergedColorsList = new List<ThemeVariantColorCollection>();
            var nullVariantColorsList = new List<ThemeVariantColorCollection>();

            foreach (var themeColor in themeColors)
            {
                var needNewCollection = true;

                if (themeColor.VariantName == null)
                {
                    foreach (var nullVariantColors in nullVariantColorsList)
                    {
                        if (string.Equals(nullVariantColors.ThemeName, themeColor.ThemeName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!nullVariantColors.Contains(themeColor))
                            {
                                nullVariantColors.Add(themeColor);
                            }

                            needNewCollection = false;

                            break;
                        }
                    }

                    if (needNewCollection)
                    {
                        nullVariantColorsList.Add(new ThemeVariantColorCollection(themeColor.ThemeName, themeColor.VariantName)
                        {
                            themeColor
                        });
                    }
                }
                else
                {
                    foreach (var mergedColors in mergedColorsList)
                    {
                        if (string.Equals(mergedColors.ThemeName, themeColor.ThemeName, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(mergedColors.VariantName, themeColor.VariantName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!mergedColors.Contains(themeColor))
                            {
                                mergedColors.Add(themeColor);
                            }

                            needNewCollection = false;

                            break;
                        }
                    }

                    if (needNewCollection)
                    {
                        mergedColorsList.Add(new ThemeVariantColorCollection(themeColor.ThemeName, themeColor.VariantName)
                        {
                            themeColor
                        });
                    }
                }
            }

            foreach (var nullVariantColors in nullVariantColorsList)
            {
                foreach (var nullVariantColor in nullVariantColors)
                {
                    foreach (var mergedColors in mergedColorsList)
                    {
                        mergedColors.Add(nullVariantColor);
                    }
                }
            }

            return mergedColorsList;
        }

        private static object? FindThemeResource(ResourceDictionary themeDictionary, string resourceKey)
        {
            foreach (var kvp in themeDictionary)
            {
                if (kvp.Key == null)
                {
                    System.Diagnostics.Trace.WriteLine($"Key was null.");
                    continue;
                }

                if (kvp.Key.ToString() == resourceKey)
                {
                    var value = themeDictionary[kvp.Key];

                    return value;
                }
            }

            return null;
        }

        private static Color? FindThemeColor(ResourceDictionary themeDictionary, string resourceKey)
        {
            var resource = FindThemeResource(themeDictionary, resourceKey);

            if (resource is not null)
            {
                if (resource is Color color)
                {
                    return color;
                }
                else if (resource is SolidColorBrush solidColorBrush)
                {
                    return solidColorBrush.Color;
                }
            }

            System.Diagnostics.Trace.WriteLine($"Key '{resourceKey}' retrieved a value that was an unknown type '{resource?.GetType()}'.");

            return null;
        }
    }
}
