using System;
using System.Collections.Generic;
using System.Windows.Media;

using Core.Events;

namespace Core.ViewModel
{
    public static class FactionsEngine
    {
        static Dictionary<string, FactionViewModel> _knownFactions = new Dictionary<string, FactionViewModel>();

        public static void Start(FiltersEvent filtersEvent)
        {
            FactionsUpdatedWeakEventManager.AddHandler(filtersEvent, OnFactionFiltersUpdated);
        }

        static void OnFactionFiltersUpdated(object sender, EventArgs e)
        {
            foreach (var (id, vm) in _knownFactions)
            {
                var fi = Model.Filters.ExpandFaction(id);

                if (fi == null)
                {
                    UpdateToUnknown(vm, id);
                }
                else
                {
                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fi.Color));
                    var geometry = Geometry.Parse(fi.Logo);
                    vm.UpdateTo(fi.Name, brush, geometry);
                }
            }
        }

        static FactionViewModel TryCreateNew(string factionId)
        {
            var fi = Model.Filters.ExpandFaction(factionId);
            if (fi == null)
                return null;
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fi.Color));
            var geometry = Geometry.Parse(fi.Logo);
            return new FactionViewModel(fi.Name, brush, geometry);
        }

        static FactionViewModel CreateUnknown(string name) =>
            new FactionViewModel(name, Brushes.Black, new EllipseGeometry(new System.Windows.Point(), 1, 1));

        static void UpdateToUnknown(FactionViewModel vm, string name) =>
            vm.UpdateTo(name, Brushes.Black, new EllipseGeometry(new System.Windows.Point(), 1, 1));

        internal static FactionViewModel ById(string factionId)
        {
            if (factionId == null) return null;
            if (_knownFactions.TryGetValue(factionId, out var knownFaction))
                return knownFaction;
            var newFaction = TryCreateNew(factionId) ?? CreateUnknown(factionId);
            _knownFactions.Add(factionId, newFaction);
            return newFaction;
        }
    }
}
