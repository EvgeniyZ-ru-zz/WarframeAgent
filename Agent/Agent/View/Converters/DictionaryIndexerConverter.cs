using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Agent.ViewModel;

namespace Agent.View.Converters
{
    class DictionaryIndexerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == DependencyProperty.UnsetValue)
                return Binding.DoNothing;
            if (values == null)
                throw new ArgumentException("Expect non-null value collection");
            if (values.Length != 2)
                throw new ArgumentException("Expect 2 values");
            if (values[0] == null || values[0] == null)
                return Binding.DoNothing;
            var dict = (System.Collections.IDictionary)values[0];
            var key = values[1];
            return dict[key];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
