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
            Faction = invasion.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            NodeArray = invasion.Node.GetFilter(Model.Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            LocTag = invasion.LocTag.GetFilter(Model.Filters.FilterType.Mission).FirstOrDefault().Key;
            DefenderReward = GetRewardString(invasion.DefenderReward);
            AttackerReward = GetRewardString(invasion.AttackerReward);
            AttackerColor = GetFactionColor(invasion.AttackerMissionInfo.Faction);
            DefenderColor = GetFactionColor(invasion.DefenderMissionInfo.Faction);
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
        public string Faction { get; }
        public string[] NodeArray { get; }
        public string DefenderFaction { get; }
        public string AttackerFaction { get; }
        public Brush DefenderColor { get; }
        public Brush AttackerColor { get; }
        public string DefenderReward { get; }
        public string AttackerReward { get; }

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

        static Dictionary<string, Brush> _factionColors = new Dictionary<string, Brush>()
        {
            ["FC_GRINEER"]     = new SolidColorBrush(Color.FromRgb(r: 0xbd, g: 0x58, b: 0x57)), //FFbd5857
            ["FC_CORPUS"]      = new SolidColorBrush(Color.FromRgb(r: 0x3f, g: 0x85, b: 0xca)), //FF3f85ca
            ["FC_INFESTATION"] = new SolidColorBrush(Color.FromRgb(r: 0x1f, g: 0x97, b: 0x45))  //FF1F9745
        };
        static Brush GetFactionColor(string faction) =>
            _factionColors.TryGetValue(faction, out var brush) ? brush : null;

        static string GetRewardString(InvasionReward reward)
        {
            var rewardString = reward?.CountedItems[0]?.ItemType.GetFilter(Model.Filters.FilterType.Item).FirstOrDefault().Key;
            var count = reward?.CountedItems[0]?.ItemCount;
            if (count > 1)
                rewardString += $" [{count}]";
            return rewardString;
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
