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

        static Dictionary<string, FactionViewModel> _knownFactions = new Dictionary<string, FactionViewModel>();

        static FactionViewModel TryCreateNew(string factionId)
        {
            var fiOrNull = Model.Filters.TryExpandFaction(factionId);
            if (fiOrNull == null)
                return null;
            var fi = fiOrNull.Value;
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fi.color));
            var geometry = Geometry.Parse(fi.logo);
            return new FactionViewModel(fi.name, brush, geometry);
        }

        static FactionViewModel CreateUnknown(string name) =>
            new FactionViewModel(name, Brushes.Black, new EllipseGeometry(new System.Windows.Point(), 1, 1));

        public static FactionViewModel ById(string factionId)
        {
            if (_knownFactions.TryGetValue(factionId, out var knownFaction))
                return knownFaction;
            var newFaction = TryCreateNew(factionId) ?? CreateUnknown(factionId);
            _knownFactions.Add(factionId, newFaction);
            return newFaction;
        }

        private FactionViewModel(string name, Brush color, Geometry logo)
        {
            Name = name;
            Color = color;
            Logo = logo;
        }
    }
}
