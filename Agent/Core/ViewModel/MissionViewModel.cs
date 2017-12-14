using System;
using System.Windows;
using System.Windows.Media;

using Core.Model;
using Core.Events;

namespace Core.ViewModel
{
    public class MissionViewModel : VM
    {
        string reward;
        MissionInfo missionInfo;

        public int MinEnemyLevel { get; }
        public int MaxEnemyLevel { get; }
        public string Reward { get => reward; private set => Set(ref reward, value); } // может поменяться при обновлении фильтров
        public FactionViewModel Faction { get; }
        public SectorViewModel Sector { get; }
        public string MissionType { get; }
        public Brush RewardColor { get; }
        public Visibility ModeVisibility { get; }
        public Visibility NightmareVisibility { get; }
        public string ModeIcon { get; }
        public string ModeToolTip { get; }
        public Visibility RewardVisibility { get; }
        public Visibility CreditVisibility { get; }
        public MissionReward MissionReward { get; } //? exposing model class to UI?

        public MissionViewModel(MissionInfo missionInfo, FiltersEvent filtersEvent)
        {
            var (rewardType, rewardValue) = GetRewardProperties(missionInfo);
            Reward = rewardValue;
            RewardColor = GetBrushForReward(rewardType);
            Faction = FactionViewModel.ById(missionInfo.Faction);
            Sector = SectorViewModel.FromSector(missionInfo.Location);
            MissionType = Model.Filters.ExpandMission(missionInfo.MissionType)?.Name ?? missionInfo.MissionType;

            if (missionInfo.ArchwingRequired == true || missionInfo.IsSharkwingMission == false)
            {
                ModeIcon = "PaperPlaneOutline";
                ModeToolTip = "Арчвинг миссия";
                ModeVisibility = Visibility.Visible;
            }
            else if (missionInfo.IsSharkwingMission == true)
            {
                ModeIcon = "SnowflakeOutline";
                ModeToolTip = "Шарквинг миссия";
                ModeVisibility = Visibility.Visible;
            }

            NightmareVisibility = missionInfo.Nightmare == true
                ? Visibility.Visible
                : Visibility.Collapsed;

            RewardVisibility = (missionInfo.MissionReward.CountedItems != null) || (missionInfo.MissionReward.Items != null)
                ? Visibility.Visible
                : Visibility.Collapsed;
            CreditVisibility = (missionInfo.MissionReward.Credits > 0) ? Visibility.Visible : Visibility.Collapsed;
            MinEnemyLevel = missionInfo.MinEnemyLevel;
            MaxEnemyLevel = missionInfo.MaxEnemyLevel;
            MissionReward = missionInfo.MissionReward;

            this.missionInfo = missionInfo;
            ItemsUpdatedWeakEventManager.AddHandler(filtersEvent, OnItemsFilterUpdated);
        }

        void OnItemsFilterUpdated(object sender, EventArgs args)
        {
            var (rewardType, rewardValue) = GetRewardProperties(missionInfo);
            Reward = rewardValue;
        }

        static (string rewardType, string rewardValue) GetRewardProperties(MissionInfo missionInfo)
        {
            string rewardKey = null;
            string itemCount = null;
            if (missionInfo.MissionReward.CountedItems != null)
            {
                var item = missionInfo.MissionReward.CountedItems[0];
                itemCount = item.ItemCount >= 2 ? $" [{item.ItemCount}]" : string.Empty;
                rewardKey = item.ItemType;
            }
            else if (missionInfo.MissionReward.Items != null)
            {
                rewardKey = missionInfo.MissionReward.Items[0];
            }

            var reward = Model.Filters.ExpandItem(rewardKey);
            string rewardValue = reward?.Value ?? rewardKey;
            if (itemCount != null)
                rewardValue += itemCount;
            return (rewardType: reward?.Type, rewardValue: rewardValue);
        }

        static Brush GetBrushForReward(string rewardType)
        {
            switch (rewardType) //TODO: Переделать на универсальный тип (если будет смена языка, то названия не будут на русском).
            {
            case "Шлема":
                return Brushes.YellowGreen;
            case "Чертежи":
                return Brushes.BlueViolet;
            case "Ауры":
                return Brushes.OrangeRed;
            case "Модификаторы":
                return Brushes.DarkCyan;
            default:
                return null;
            }
        }
    }
}
