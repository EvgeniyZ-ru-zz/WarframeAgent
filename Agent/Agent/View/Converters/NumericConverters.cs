using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Agent.View.Converters
{
    class ConstantMultiplyingLongConverter : IValueConverter
    {
        public long Multiplier { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (long)value * Multiplier;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (long)value / Multiplier;
    }

    class VisibleIfAtLeastConverter : IValueConverter
    {
        public long LowerBound { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            System.Convert.ToInt64(value) > LowerBound ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
