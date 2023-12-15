using Avalonia;
using AvaloniaThemeDemo.Models;
using AvaloniaThemeDemo.Services;
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

        public IThemeResourceProvider ThemeResourceProvider { get; }

        [ObservableProperty]
        private List<ThemeVariantColorCollection> _themes = new();

        [ObservableProperty]
        private int _themeNameSelectedIndex = 0;

        [ObservableProperty]
        private List<ThemeColor> _themeColors = new();

        [ObservableProperty]
        private string? _currentThemeVariantName;

        [ObservableProperty]
        private string? _searchPattern;

        public MainWindowViewModel(IThemeResourceProvider themeResourceProvider)
        {
            ThemeResourceProvider = themeResourceProvider;

            PropertyChanged += MainWindowViewModel_PropertyChanged;

            if (Application.Current?.Styles == null)
            {
                System.Diagnostics.Trace.WriteLine($"Application does not have any styles.");
                return;
            }

            Themes = ThemeResourceProvider.GetAllThemeVariantColors() ?? new List<ThemeVariantColorCollection>();

            if (ThemeNameSelectedIndex >= 0 && ThemeNameSelectedIndex < Themes.Count)
            {
                LoadThemeResources(Themes[ThemeNameSelectedIndex]);
            }

            CurrentThemeVariantName = Application.Current.ActualThemeVariant.ToString();
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThemeNameSelectedIndex))
            {
                LoadThemeResources(Themes[ThemeNameSelectedIndex]);
            }
            else if (e.PropertyName == nameof(SearchPattern))
            {
                ThemeColors = ApplySearchPattern(SearchPattern, _allThemeColors);
            }
        }

        private void LoadThemeResources(ThemeVariantColorCollection? themeVariantColorCollection)
        {
            if (themeVariantColorCollection == null)
            {
                return;
            }

            _allThemeColors = themeVariantColorCollection.OrderBy(i => i.Key).ToList();

            ThemeColors = ApplySearchPattern(SearchPattern, _allThemeColors);
        }

        private static List<ThemeColor> ApplySearchPattern(string? searchPattern, List<ThemeColor> allThemeColors)
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                return allThemeColors;
            }

            return allThemeColors.Where(i => FileSystemName.MatchesSimpleExpression($"*{searchPattern}*", i.Key, true)).ToList();
        }
    }
}
