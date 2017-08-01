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
        public string Faction { get; }
        public string[] Planet { get; }
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
            Faction = missionInfo.Faction.GetFilter(Model.Filters.FilterType.Fraction).FirstOrDefault().Key;
            Planet = missionInfo.Location.GetFilter(Model.Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            MissionType = missionInfo.MissionType.GetFilter(Model.Filters.FilterType.Mission).FirstOrDefault().Key;
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
                var itemCount = item.ItemCount >= 2 ? $"[{item.ItemCount}]" : string.Empty;
                var reward = item.ItemType.GetFilter(Model.Filters.FilterType.Item).FirstOrDefault();

                return (rewardType: reward.Value, rewardValue: $"{reward.Key} {itemCount}");
            }
            else if (missionInfo.MissionReward.Items != null)
            {
                var reward = missionInfo.MissionReward.Items[0].GetFilter(Model.Filters.FilterType.Item)
                    .FirstOrDefault();

                return (rewardType: reward.Value, rewardValue: reward.Key);
            }

            return (null, null);

            #endregion
        }

        static Brush GetBrushForReward(string rewardType)
        {
            switch (rewardType)
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
