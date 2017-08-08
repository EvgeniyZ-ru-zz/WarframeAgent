using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Core.ViewModel
{
    public class FactionViewModel : VM
    {
        public string Name { get; }
        public Brush Color { get; }
        public Geometry Logo { get; }

        static Dictionary<string, FactionViewModel> _knownFactions =
            Model.FiltersModel.Factions.GetAll().ToDictionary(kvp => kvp.Key, kvp => FromFactionInfo(kvp.Value));

        static FactionViewModel FromFactionInfo(Model.FiltersModel.FactionInfo value)
        {
            var color = (Color)ColorConverter.ConvertFromString(value.Color);
            var brush = new SolidColorBrush(color);
            var geometry = Geometry.Parse(value.Logo);
            return new FactionViewModel(value.Name, brush, geometry);
        }

        public static FactionViewModel ById(string factionId)
        {
            if (_knownFactions.Count == 0)
                throw new InvalidOperationException("Calling this too early? There are no factions available");
            return _knownFactions.TryGetValue(factionId, out var faction) ? faction : null;
        }

        private FactionViewModel(string name, Brush color, Geometry logo)
        {
            Name = name;
            Color = color;
            Logo = logo;
        }
    }
}
