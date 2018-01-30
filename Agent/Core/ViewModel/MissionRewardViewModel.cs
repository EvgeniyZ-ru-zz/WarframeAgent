using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Core.Events;
using Core.Model;

namespace Core.ViewModel
{
    public class MissionRewardViewModel : VM
    {
        readonly MissionReward missionReward;
        readonly string key;
        readonly string itemCountString;

        public MissionRewardViewModel(MissionReward missionReward, FiltersEvent filtersEvent)
        {
            this.missionReward = missionReward;

            if (missionReward.CountedItems != null)
            {
                var item = missionReward.CountedItems[0];
                if (item.ItemCount >= 2)
                    itemCountString = $" [{item.ItemCount}]";
                key = item.ItemType;
                Visibility = Visibility.Visible;
            }
            else if (missionReward.Items != null)
            {
                key = missionReward.Items[0];
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }

            var (rewardType, rewardValue) = GetRewardProperties();
            FilteredName = rewardValue;
            Color = GetBrushForReward(rewardType);

            CreditVisibility = (Credits > 0) ? Visibility.Visible : Visibility.Collapsed;

            ItemsUpdatedWeakEventManager.AddHandler(filtersEvent, OnItemsFilterUpdated);
        }

        private string filteredName;
        public string FilteredName { get => filteredName; private set => Set(ref filteredName, value); }

        public Brush Color { get; }
        public Visibility Visibility { get; }
        public Visibility CreditVisibility { get; }
        public int Credits => missionReward.Credits;
        public string Key => key;

        void OnItemsFilterUpdated(object sender, EventArgs args)
        {
            (_, FilteredName) = GetRewardProperties();
        }

        (string rewardType, string rewardFriendlyName) GetRewardProperties()
        {
            var rewardItem = Model.Filters.ExpandItem(key);
            string rewardFriendlyName = rewardItem?.Value ?? key;
            if (itemCountString != null)
                rewardFriendlyName += itemCountString;
            return (rewardType: rewardItem?.Type, rewardFriendlyName);
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
