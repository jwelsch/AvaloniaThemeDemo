using Avalonia.Media;
using System;

namespace AvaloniaThemeDemo.Reflection
{
    public class SystemAccentColorsAccessor
    {
        private readonly Func<Color?> _systemAccentColorFieldGetter;
        private readonly Func<Color?> _systemAccentColorDark1FieldGetter;
        private readonly Func<Color?> _systemAccentColorDark2FieldGetter;
        private readonly Func<Color?> _systemAccentColorDark3FieldGetter;
        private readonly Func<Color?> _systemAccentColorLight1FieldGetter;
        private readonly Func<Color?> _systemAccentColorLight2FieldGetter;
        private readonly Func<Color?> _systemAccentColorLight3FieldGetter;

        public object Instance { get; }

        public Color? SystemAccentColor => _systemAccentColorFieldGetter.Invoke();

        public Color? SystemAccentColorDark1 => _systemAccentColorDark1FieldGetter.Invoke();

        public Color? SystemAccentColorDark2 => _systemAccentColorDark2FieldGetter.Invoke();

        public Color? SystemAccentColorDark3 => _systemAccentColorDark3FieldGetter.Invoke();

        public Color? SystemAccentColorLight1 => _systemAccentColorLight1FieldGetter.Invoke();

        public Color? SystemAccentColorLight2 => _systemAccentColorLight2FieldGetter.Invoke();

        public Color? SystemAccentColorLight3 => _systemAccentColorLight3FieldGetter.Invoke();

        public SystemAccentColorsAccessor(object instance)
        {
            Instance = instance;

            var binder = Binder.Bind(Instance).AsType("Avalonia.Themes.Fluent.Accents.SystemAccentColors");

            _systemAccentColorFieldGetter = binder.ToField<Color?>("_systemAccentColor");
            _systemAccentColorDark1FieldGetter = binder.ToField<Color?>("_systemAccentColorDark1");
            _systemAccentColorDark2FieldGetter = binder.ToField<Color?>("_systemAccentColorDark2");
            _systemAccentColorDark3FieldGetter = binder.ToField<Color?>("_systemAccentColorDark3");
            _systemAccentColorLight1FieldGetter = binder.ToField<Color?>("_systemAccentColorDark1");
            _systemAccentColorLight2FieldGetter = binder.ToField<Color?>("_systemAccentColorLight2");
            _systemAccentColorLight3FieldGetter = binder.ToField<Color?>("_systemAccentColorLight3");
        }
    }
}
