using System;
using System.Windows;
using System.Windows.Data;

namespace Agent.View.Converters
{
    public class CountToInvisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = System.Convert.ToInt32(value);
            var visibility = count <= 0 ? Visibility.Visible : Visibility.Collapsed;

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
