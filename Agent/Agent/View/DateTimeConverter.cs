using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Agent.View
{
    class DateTimeConverter : IValueConverter
    {
        static readonly DateTime origin = new DateTime(2000, 1, 1);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine($"input {(DateTime)value}, out {((DateTime)value - origin).TotalSeconds}");
            return ((DateTime)value - origin).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            origin + TimeSpan.FromSeconds((double)value);
    }
}
