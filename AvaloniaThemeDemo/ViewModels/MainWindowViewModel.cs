using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaThemeDemo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Enumeration;
using System.Linq;

namespace AvaloniaThemeDemo.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private List<ThemeColor> _allThemeColors = new();

        [ObservableProperty]
        private List<ThemeInfo> _themes = new();

        [ObservableProperty]
        private int _themeNameSelectedIndex = 0;

        [ObservableProperty]
        private List<ThemeColor> _themeColors = new();

        [ObservableProperty]
        private string? _currentThemeName;

        [ObservableProperty]
        private string? _searchPattern;

        public MainWindowViewModel()
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;

            if (Application.Current?.Styles == null)
            {
                System.Diagnostics.Trace.WriteLine($"Application does not have any styles.");
                return;
            }

            var themes = new List<ThemeInfo>();
            var index = 0;

            foreach (var style in Application.Current.Styles)
            {
                var styleType = style.GetType();

                if (style is Styles theStyle)
                {
                    foreach (var themeDictionary in theStyle.Resources.ThemeDictionaries)
                    {
                        var themeName = $"{styleType.Name.Replace("Theme", "")} {themeDictionary.Key}";
                        var themeInfo = new ThemeInfo(index++, themeName, themeDictionary.Value);
                        themes.Add(themeInfo);
                    }
                }
            }

            Themes = themes;

            if (ThemeNameSelectedIndex >= 0 && ThemeNameSelectedIndex < Themes.Count)
            {
                LoadThemeResources(Themes[ThemeNameSelectedIndex].ThemeVariantProvider);
            }

            CurrentThemeName = Application.Current.ActualThemeVariant.ToString();
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThemeNameSelectedIndex))
            {
                LoadThemeResources(Themes[ThemeNameSelectedIndex].ThemeVariantProvider);
            }
            else if (e.PropertyName == nameof(SearchPattern))
            {
                ThemeColors = ApplySearchPattern(SearchPattern, _allThemeColors);
            }
        }

        private void LoadThemeResources(IThemeVariantProvider themeVariantProvider)
        {
            if (themeVariantProvider is not ResourceDictionary themeDictionary)
            {
                System.Diagnostics.Trace.WriteLine($"Theme variant provider was not of type '{nameof(ResourceDictionary)}'.");
                return;
            }

            var themeColors = new List<ThemeColor>();

            foreach (var kvp in themeDictionary)
            {
                if (kvp.Key == null)
                {
                    System.Diagnostics.Trace.WriteLine($"Key was null.");
                    continue;
                }

                var value = themeDictionary[kvp.Key];

                var valueType = value?.GetType();

                Color valueColor;

                if (value is null)
                {
                    System.Diagnostics.Trace.WriteLine($"Key '{kvp.Key}' retrieved a null value.");
                    continue;
                }
                else if (value is Color color)
                {
                    valueColor = color;
                }
                else if (value is SolidColorBrush solidColorBrush)
                {
                    valueColor = solidColorBrush.Color;
                }
                else
                {
                    //System.Diagnostics.Trace.WriteLine($"Key '{kvp.Key}' retrieved a value that was an unknown type '{value.GetType()}'.");
                    continue;
                }

                var name = kvp.Key.ToString() ?? "<null>";
                var hexValue = $"#{valueColor.R:X2}{valueColor.G:X2}{valueColor.B:X2}";
                var themeColor = new ThemeColor(name, valueColor);

                //System.Diagnostics.Trace.WriteLine($"Hex value: {hexValue}");

                themeColors.Add(themeColor);
            }

            _allThemeColors = themeColors.OrderBy(i => i.Name).ToList();

            ThemeColors = ApplySearchPattern(SearchPattern, _allThemeColors);
        }

        private static List<ThemeColor> ApplySearchPattern(string? searchPattern, List<ThemeColor> allThemeColors)
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                return allThemeColors;
            }

            return allThemeColors.Where(i => FileSystemName.MatchesSimpleExpression($"*{searchPattern}*", i.Name, true)).ToList();
        }
    }
}
