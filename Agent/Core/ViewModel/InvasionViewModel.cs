using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
            AttackerFaction = FactionViewModel.ById(invasion.AttackerMissionInfo.Faction);
            DefenderFaction = FactionViewModel.ById(invasion.DefenderMissionInfo.Faction);
            Faction = FactionViewModel.ById(invasion.Faction);
            Sector = SectorViewModel.FromSector(invasion.Node);
            LocTag = Model.Filters.ExpandMission(invasion.LocTag)?.Name ?? invasion.LocTag;
            var defRew = GetRewardString(invasion.DefenderReward);
            var atkRew = GetRewardString(invasion.AttackerReward);
            DefenderReward = defRew.value;
            AttackerReward = atkRew.value;
            AttackerRewardCount = atkRew.count;
            DefenderRewardCount = defRew.count;
            Update();
        }

        public void Update()
        {
            Count = invasion.Count;
            Goal = invasion.Goal;
            UpdatePercent();
        }

        public Id Id { get; }
        public string LocTag { get; }
        public FactionViewModel Faction { get; }
        public SectorViewModel Sector { get; }
        public FactionViewModel DefenderFaction { get; }
        public FactionViewModel AttackerFaction { get; }
        public string DefenderReward { get; }
        public string AttackerReward { get; }
        public string AttackerRewardCount { get; }
        public string DefenderRewardCount { get; }

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

        static (string value, string count) GetRewardString(InvasionReward reward)
        {
            var item0 = reward?.CountedItems[0];
            var item0Type = item0?.ItemType;
            var itemCount = "";
            var expandedReward = Model.Filters.ExpandItem(item0Type)?.Value ?? item0Type;
            var count = item0?.ItemCount;
            if (count > 1)
                itemCount= $" [{count}]";

            return (value: string.IsNullOrEmpty(expandedReward) ? "Недоступно" : expandedReward, count: itemCount);
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
