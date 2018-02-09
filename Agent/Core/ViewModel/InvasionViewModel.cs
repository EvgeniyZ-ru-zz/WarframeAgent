using System;
using Core.Events;
using Core.Model;

namespace Core.ViewModel
{
    public class InvasionViewModel : VM, IUpdatable
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
            DefenderReward = new InvasionRewardViewModel(invasion.DefenderReward, filtersEvent);
            AttackerReward = new InvasionRewardViewModel(invasion.AttackerReward, filtersEvent);
            Update();
            SectorsUpdatedWeakEventManager.AddHandler(filtersEvent, OnSectorsFilterUpdated);
            MissionsUpdatedWeakEventManager.AddHandler(filtersEvent, OnMissionsFilterUpdated);
        }

        private void OnSectorsFilterUpdated(object sender, EventArgs eventArgs) =>
            Sector = SectorViewModel.FromSector(invasion.Node);

        private void OnMissionsFilterUpdated(object sender, EventArgs eventArgs) =>
            LocTag = Model.Filters.ExpandMission(invasion.LocTag)?.Name ?? invasion.LocTag;

        public void Update()
        {
            Count = invasion.Count;
            Goal = invasion.Goal;
            UpdatePercent();
        }

        public Id Id { get; }
        private string locTag;
        public string LocTag { get => locTag; private set => Set(ref locTag, value); }

        public FactionViewModel Faction { get; }

        private SectorViewModel sector;
        public SectorViewModel Sector { get => sector; set => Set(ref sector, value); }
        public FactionViewModel DefenderFaction { get; }
        public FactionViewModel AttackerFaction { get; }

        public InvasionRewardViewModel DefenderReward { get; }
        public InvasionRewardViewModel AttackerReward { get; }

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

            if (val > 100) val = 100;
            if (val < 0) val = 0;

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
