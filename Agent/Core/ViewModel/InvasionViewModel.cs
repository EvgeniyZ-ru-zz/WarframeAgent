using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Model;

namespace Core.ViewModel
{
    public class InvasionViewModel : VM
    {
        private Invasion invasion;

        public InvasionViewModel(Invasion invasion)
        {
            this.invasion = invasion;
            Id = invasion.Id;
            isDefenderFactionInfestation = invasion.DefenderMissionInfo.Faction == "FC_INFESTATION";
            AttackerFaction = invasion.AttackerMissionInfo.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            DefenderFaction = invasion.DefenderMissionInfo.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            Faction = invasion.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            NodeArray = invasion.Node.GetFilter(Model.Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            Update();
        }

        public void Update()
        {
            Count = invasion.Count;
            Goal = invasion.Goal;
            UpdatePercent();
        }

        public Id Id { get; }
        public string Faction { get; }
        public string[] NodeArray { get; }
        public string DefenderFaction { get; }
        public string AttackerFaction { get; }

        private readonly bool isDefenderFactionInfestation;

        private double _count;
        public double Count
        {
            get => _count;
            private set => Set(ref _count, value);
        }

        private double _goal;
        public double Goal
        {
            get => _goal;
            private set => Set(ref _goal, value);
        }

        void UpdatePercent()
        {
            var val = isDefenderFactionInfestation
                    ? (Goal + Count) / Goal * 100
                    : (Goal + Count) / (Goal * 2) * 100;

            Percent = val;
            PercentOut = 100 - val;
        }

        private double _percentOut;
        public double PercentOut
        {
            get => _percentOut;
            private set => Set(ref _percentOut, value);
        }

        private double _percent;

        public double Percent
        {
            get => _percent;
            private set => Set(ref _percent, value);
        }
    }
}
