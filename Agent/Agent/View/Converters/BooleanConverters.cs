using System;
using System.Globalization;
using System.Windows.Data;

namespace Agent.View.Converters
{
    class BooleanConverter<T> : IValueConverter
    {
        public T OnTrue { get; set; }
        public T OnFalse { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? OnTrue : OnFalse;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Equals(value, OnTrue);
    }
}
