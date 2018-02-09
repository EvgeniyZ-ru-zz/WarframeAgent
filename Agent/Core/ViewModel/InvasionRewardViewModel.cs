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
    public class InvasionRewardViewModel : VM
    {
        readonly InvasionReward invasionReward;
        readonly string key;

        public InvasionRewardViewModel(InvasionReward invasionReward, FiltersEvent filtersEvent)
        {
            this.invasionReward = invasionReward;

            if (invasionReward?.CountedItems != null)
            {
                var item = invasionReward.CountedItems[0];
                key = item.ItemType;
                Count = invasionReward.CountedItems.Count;
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }

            var (rewardType, rewardValue) = GetRewardProperties();
            FilteredName = rewardValue;
            Color = GetBrushForReward(rewardType);

            ItemsUpdatedWeakEventManager.AddHandler(filtersEvent, OnItemsFilterUpdated);
        }

        private string filteredName;
        public string FilteredName { get => filteredName; private set => Set(ref filteredName, value); }

        public Brush Color { get; }
        public Visibility Visibility { get; }
        public int Count { get; }

        public string Key => key;

        void OnItemsFilterUpdated(object sender, EventArgs args)
        {
            (_, FilteredName) = GetRewardProperties();
        }

        (string rewardType, string rewardFriendlyName) GetRewardProperties()
        {
            var rewardItem = Model.Filters.ExpandItem(key);
            string rewardFriendlyName = rewardItem?.Value ?? key ?? "Недоступно";
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
