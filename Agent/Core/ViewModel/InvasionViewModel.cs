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
            AttackerFaction = invasion.AttackerMissionInfo.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            DefenderFaction = invasion.DefenderMissionInfo.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            AttackerFactionDefault = invasion.AttackerMissionInfo.Faction;
            DefenderFactionDefault = invasion.DefenderMissionInfo.Faction;
            Faction = invasion.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            NodeArray = invasion.Node.GetFilter(Model.Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            LocTag = invasion.LocTag.GetFilter(Model.Filters.FilterType.Mission).FirstOrDefault().Key;
            DefenderReward = invasion.DefenderReward?.CountedItems[0]?.ItemType.GetFilter(Model.Filters.FilterType.Item).FirstOrDefault().Key;
            DefenderRewardCount = invasion.DefenderReward?.CountedItems[0]?.ItemCount;
            //AttackerReward = invasion.AttackerReward?.CountedItems[0]?.ItemType.GetFilter(Model.Filters.FilterType.Item).FirstOrDefault().Key;
            //AttackerRewardCount = invasion.AttackerReward?.CountedItems[0]?.ItemCount;
            Update();
        }

        public void Update()
        {
            Count = invasion.Count;
            Goal = invasion.Goal;
            UpdatePercent();
            UpdateColor();
            UpdateReward();
        }

        public Id Id { get; }
        public string LocTag { get; }
        public string Faction { get; }
        public string[] NodeArray { get; }
        public string DefenderFaction { get; }
        public string AttackerFaction { get; }
        public string DefenderFactionDefault { get; }
        public string AttackerFactionDefault { get; }
        public Brush DefenderColor { get; set; }
        public Brush AttackerColor { get; set; }
        public string DefenderReward { get; set; }
        public string AttackerReward { get; set; }
        public int? DefenderRewardCount { get; }
        public int? AttackerRewardCount { get; }

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

        void UpdateColor()
        {
            switch (DefenderFactionDefault)
            {
                case "FC_GRINEER":
                    DefenderColor = new SolidColorBrush(Color.FromRgb(r: 0xbd, g: 0x58, b: 0x57)); //FFbd5857
                    break;
                case "FC_CORPUS":
                    DefenderColor = new SolidColorBrush(Color.FromRgb(r: 0x3f, g: 0x85, b: 0xca)); //FF3f85ca
                    break;
                case "FC_INFESTATION":
                    DefenderColor = new SolidColorBrush(Color.FromRgb(r: 0x1f, g: 0x97, b: 0x45)); //FF1F9745
                    break;
            }

            switch (AttackerFactionDefault)
            {
                case "FC_GRINEER":
                    AttackerColor = new SolidColorBrush(Color.FromRgb(r: 0xbd, g: 0x58, b: 0x57)); //FFbd5857
                    break;
                case "FC_CORPUS":
                    AttackerColor = new SolidColorBrush(Color.FromRgb(r: 0x3f, g: 0x85, b: 0xca)); //FF3f85ca
                    break;
                case "FC_INFESTATION":
                    AttackerColor = new SolidColorBrush(Color.FromRgb(r: 0x1f, g: 0x97, b: 0x45)); //FF1F9745
                    break;
            }
        }

        void UpdateReward()
        {
            if (DefenderReward != null)
            {
                if (DefenderRewardCount != null && DefenderRewardCount > 1)
                    DefenderReward += $" [{DefenderRewardCount}]";
            }
            if (AttackerReward != null)
            {
                if (AttackerRewardCount != null && AttackerRewardCount > 1)
                    AttackerReward += $" [{AttackerRewardCount}]";
            }
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
