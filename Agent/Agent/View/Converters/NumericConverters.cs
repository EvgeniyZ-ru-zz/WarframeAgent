using System;
using System.Globalization;
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
}
