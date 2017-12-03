using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Agent.View.Converters
{
    class NullConverter<T> : IValueConverter
    {
        public T OnNull { get; set; }
        public T OnNonNull { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? OnNull : OnNonNull;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    class NullToVisilityConverter : NullConverter<Visibility> { }

    class NullToStringConverter : NullConverter<string> { }
}
