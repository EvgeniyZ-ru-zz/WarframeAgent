using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Events;
using Core.Model;

namespace Core.ViewModel
{
    public class InvasionViewModel : VM
    {
        private Invasion invasion;

        public InvasionViewModel(Invasion invasion, FiltersEvent filtersEvent)
        {
            this.invasion = invasion;
            Id = invasion.Id;
            isDefenderFactionInfestation = invasion.DefenderMissionInfo.Faction == "FC_INFESTATION";
            AttackerFaction = FactionViewModel.ById(invasion.AttackerMissionInfo.Faction);
            DefenderFaction = FactionViewModel.ById(invasion.DefenderMissionInfo.Faction);
            Faction = FactionViewModel.ById(invasion.Faction);
            Sector = SectorViewModel.FromSector(invasion.Node);
            LocTag = Model.Filters.ExpandMission(invasion.LocTag)?.Name ?? invasion.LocTag;
            ConvertAndSetReward();
            Update();
            ItemsUpdatedWeakEventManager.AddHandler(filtersEvent, OnItemsFilterUpdated);
        }

        void OnItemsFilterUpdated(object sender, EventArgs args) => ConvertAndSetReward();

        void ConvertAndSetReward()
        {
            var defRew = GetRewardString(invasion.DefenderReward);
            var atkRew = GetRewardString(invasion.AttackerReward);
            DefenderReward = defRew.value;
            AttackerReward = atkRew.value;
            AttackerRewardCount = atkRew.count;
            DefenderRewardCount = defRew.count;
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

        string defenderReward, attackerReward, attackerRewardCount, defenderRewardCount;
        public string DefenderReward { get => defenderReward; private set => Set(ref defenderReward, value); }
        public string DefenderRewardCount { get => defenderRewardCount; private set => Set(ref defenderRewardCount, value); }
        public string AttackerReward { get => attackerReward; private set => Set(ref attackerReward, value); }
        public string AttackerRewardCount { get => attackerRewardCount; private set => Set(ref attackerRewardCount, value); }

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
                itemCount = $" [{count}]";

            return (value: string.IsNullOrEmpty(expandedReward) ? "Недоступно" : expandedReward, count: itemCount);
        }

        void UpdatePercent()
        {
            var val = isDefenderFactionInfestation
                    ? (Goal + Count) / Goal * 100
                    : (Goal + Count) / (Goal * 2) * 100;

            if (val > 100) val = 100;

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
