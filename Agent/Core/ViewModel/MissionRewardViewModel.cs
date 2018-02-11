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

        public MissionRewardViewModel(MissionReward missionReward, IItemStore itemStore)
        {
            this.missionReward = missionReward;

            if (missionReward.CountedItems != null)
            {
                var countedItem = missionReward.CountedItems[0];
                if (countedItem.ItemCount >= 2)
                    ItemCount = countedItem.ItemCount;
                Item = itemStore.GetItemById(countedItem.ItemType);
                Visibility = Visibility.Visible;
            }
            else if (missionReward.Items != null)
            {
                Item = itemStore.GetItemById(missionReward.Items[0]);
                ItemCount = 1;
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }

            CreditVisibility = (Credits > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility Visibility { get; }
        public Visibility CreditVisibility { get; }
        public int Credits => missionReward.Credits;
        public ItemViewModel Item { get; }
        public int ItemCount { get; }
    }
}
