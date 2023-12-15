using System;
using System.Reflection;

namespace AvaloniaThemeDemo.Reflection
{
    public static class Binder
    {
        public static ChainedBinder Bind(object instance)
        {
            return new ChainedBinder(instance);
        }
    }

    public class ChainedBinder
    {
        private readonly object _instance;

        private Type? _type;

        public ChainedBinder(object instance)
        {
            _instance = instance;
        }

        public ChainedBinder AsType(string? typeFullNameCheck = null)
        {
            var type = _instance.GetType();

            if (typeFullNameCheck != null
                && !string.Equals(type.FullName, typeFullNameCheck, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Expected type with full name '{typeFullNameCheck}', but instance was of type '{type}'.");
            }

            _type = _instance.GetType();

            return this;
        }

        public Func<TReturn?> ToField<TReturn>(string fieldName, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (_type == null)
            {
                throw new InvalidOperationException($"The type was null. Make sure AsType() is called first.");
            }

            var field = _type.GetField(fieldName, flags);
            return () => (TReturn?)field?.GetValue(_instance);
        }

        public Func<TReturn?> ToProperty<TReturn>(string propertyName, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (_type == null)
            {
                throw new InvalidOperationException($"The type was null. Make sure AsType() is called first.");
            }

            var property = _type.GetProperty(propertyName, flags);
            return () => (TReturn?)property?.GetValue(_instance);
        }
    }
}
