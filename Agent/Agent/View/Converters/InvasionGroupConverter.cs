using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Core.ViewModel;

namespace Agent.View.Converters
{
    public class InvasionGroupConverter : IValueConverter
    {
        public class InvasionGroup : IEquatable<InvasionGroup>
        {
            public string Planet { get; set; }
            public string LocTag { get; set; }

            public override bool Equals(object obj) => Equals(obj as InvasionGroup);

            public override int GetHashCode()
            {
                var result = 13;
                if (Planet != null)
                    result = 29 * result + Planet.GetHashCode();
                if (LocTag != null)
                    result = 29 * result + LocTag.GetHashCode();
                return result;
            }

            public bool Equals(InvasionGroup other) => other != null && other.Planet == Planet && other.LocTag == LocTag;

            public override string ToString() => $"{Planet}: {LocTag}";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = (InvasionViewModel)value;
            return new InvasionGroup() { Planet = vm.Sector.Planet, LocTag = vm.LocTag };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
