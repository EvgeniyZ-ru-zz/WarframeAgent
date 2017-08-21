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
    public class MissionViewModel : VM
    {
        public int MinEnemyLevel { get; }
        public int MaxEnemyLevel { get; }
        public string Reward { get; }
        public FactionViewModel Faction { get; }
        public SectorViewModel Sector { get; }
        public string MissionType { get; }
        public Brush RewardColor { get; }
        public Visibility ArchvingVisibility { get; }
        public Visibility SharkwingVisibility { get; }
        public Visibility RewardVisibility { get; }
        public Visibility CreditVisibility { get; }
        public MissionReward MissionReward { get; } //? exposing model class to UI?

        public MissionViewModel(MissionInfo missionInfo)
        {
            var (rewardType, rewardValue) = GetRewardProperties(missionInfo);
            Reward = rewardValue;
            RewardColor = GetBrushForReward(rewardType);
            Faction = FactionViewModel.ById(missionInfo.Faction);
            Sector = SectorViewModel.FromSector(missionInfo.Location);
            MissionType = Model.Filters.ExpandMission(missionInfo.MissionType);
            ArchvingVisibility = (missionInfo.ArchwingRequired != true) || (missionInfo.IsSharkwingMission == true)
                ? Visibility.Collapsed
                : Visibility.Visible;
            SharkwingVisibility = (missionInfo.IsSharkwingMission == true)
                ? Visibility.Visible
                : Visibility.Collapsed;
            RewardVisibility = (missionInfo.MissionReward.CountedItems != null) || (missionInfo.MissionReward.Items != null)
                ? Visibility.Visible
                : Visibility.Collapsed;
            CreditVisibility = (missionInfo.MissionReward.Credits > 0) ? Visibility.Visible : Visibility.Collapsed;
            MinEnemyLevel = missionInfo.MinEnemyLevel;
            MaxEnemyLevel = missionInfo.MaxEnemyLevel;
            MissionReward = missionInfo.MissionReward;
        }

        static (string rewardType, string rewardValue) GetRewardProperties(MissionInfo missionInfo)
        {
            #region Переводим предмет

            if (missionInfo.MissionReward.CountedItems != null)
            {
                var item = missionInfo.MissionReward.CountedItems[0];
                var itemCount = item.ItemCount >= 2 ? $" [{item.ItemCount}]" : string.Empty;
                var reward = Model.Filters.ExpandItem(item.ItemType);

                return (rewardType: reward.type, rewardValue: $"{reward.value}{itemCount}");
            }
            else if (missionInfo.MissionReward.Items != null)
            {
                var reward = Model.Filters.ExpandItem(missionInfo.MissionReward.Items[0]);

                return (rewardType: reward.type, rewardValue: reward.value);
            }

            return (null, null);

            #endregion
        }

        static Brush GetBrushForReward(string rewardType)
        {
            switch (rewardType) //TODO: Переделать на универсальный тип (если будет смена языка, то названия не будут на русском).
            {
            case "Шлема":
                return Brushes.BlueViolet;
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
