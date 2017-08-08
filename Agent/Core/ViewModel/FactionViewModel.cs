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

        static FactionViewModel CreateUnknown(string name) =>
            new FactionViewModel(name, Brushes.Black, new EllipseGeometry(new System.Windows.Point(), 1, 1));

        public static FactionViewModel ById(string factionId) =>
            _knownFactions.TryGetValue(factionId, out var faction) ? faction : CreateUnknown(factionId);

        private FactionViewModel(string name, Brush color, Geometry logo)
        {
            Name = name;
            Color = color;
            Logo = logo;
        }
    }
}
