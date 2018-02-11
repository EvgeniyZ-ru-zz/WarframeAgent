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

        public InvasionRewardViewModel(InvasionReward invasionReward, IItemStore itemStore)
        {
            this.invasionReward = invasionReward;

            if (invasionReward?.CountedItems != null)
            {
                var countedItem = invasionReward.CountedItems[0];
                Item = itemStore.GetItemById(countedItem.ItemType);
                ItemCount = invasionReward.CountedItems.Count;
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        public Visibility Visibility { get; }
        public ItemViewModel Item { get; }
        public int ItemCount { get; }
    }
}
