using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agent.Data.Filters
{
    [ValueConversion(typeof(string[]), typeof(string))]
    class PlanetConvert : IValueConverter
    {
        private string _item;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (_item != null) return _item;

            string[] planet;
            Debug.WriteLine($"Переводим значение {value}!", $"[{DateTime.Now}]");
            try
            {
                using (StreamReader r = new StreamReader($"{Settings.Program.Directories.Data}/Filters/Planets.json"))
                {
                    var json = r.ReadToEnd();
                    var data = (JObject)JsonConvert.DeserializeObject(json);
                    planet = System.Convert.ToString(data["Items"].First[value]).Split('|');
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return value;
            }

            if (parameter != null && parameter.ToString() == "Sector" && planet.Length >= 2)
            {
                _item = planet[1].ToUpper();
                return _item;
            }
            if (parameter != null && parameter.ToString() == "Planet" && planet.Length >= 2)
            {
                _item = planet[0].ToUpper();
                return _item;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
