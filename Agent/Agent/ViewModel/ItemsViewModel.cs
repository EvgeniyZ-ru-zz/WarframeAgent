using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agent.ViewModel.Util;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class ItemsViewModel : VM
    {
        private GameViewModel GameView;

        public ObservableCollection<ItemViewModel> Items => GameView.Items;

        BatchedObservableCollection<ExtendedItemViewModel> extendedItems = new BatchedObservableCollection<ExtendedItemViewModel>();
        public ObservableCollection<ExtendedItemViewModel> ExtendedItems => extendedItems;

        public ItemsViewModel(GameViewModel gameView)
        {
            GameView = gameView;
            Items.CollectionChanged += OnItemsUpdated;
            extendedItems.AddRange(Items.Select(Extend));
        }

        ExtendedItemViewModel Extend(ItemViewModel vm) => new ExtendedItemViewModel(vm, false, OnNotificationEnabled);

        void OnNotificationEnabled(ExtendedItemViewModel vm)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));
            //throw new NotImplementedException();
        }

        void OnItemsUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
                var index = e.NewStartingIndex >= 0 ? e.NewStartingIndex : extendedItems.Count;
                extendedItems.InsertRange(e.NewItems.Cast<ItemViewModel>().Select(Extend), index);
                break;
            case NotifyCollectionChangedAction.Remove:
                var old = new HashSet<ItemViewModel>(e.OldItems.Cast<ItemViewModel>());
                var itemsToRemove = extendedItems.Where(i => old.Contains(i.Item)).ToList();
                extendedItems.RemoveAll(itemsToRemove);
                break;
            case NotifyCollectionChangedAction.Replace:
                var old2 = new HashSet<ItemViewModel>(e.OldItems.Cast<ItemViewModel>());
                var itemsToRemove2 = extendedItems.Where(i => old2.Contains(i.Item)).ToList();
                var itemsToAdd2 = e.NewItems.Cast<ItemViewModel>().Select(Extend);
                extendedItems.ReplaceRange(itemsToRemove2, itemsToAdd2);
                break;
            case NotifyCollectionChangedAction.Move:
                var old3 = new HashSet<ItemViewModel>(e.OldItems.Cast<ItemViewModel>());
                var itemsToMove = extendedItems.Where(i => old3.Contains(i.Item)).ToList();
                extendedItems.MoveRange(itemsToMove, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                extendedItems.Reset(Items.Select(Extend));
                break;
            }
        }
    }

    class ExtendedItemViewModel : VM
    {
        internal ExtendedItemViewModel(
            ItemViewModel item, bool isNotificationEnabled, Action<ExtendedItemViewModel> notificationEnabledCallback)
        {
            Item = item;
            this.isNotificationEnabled = isNotificationEnabled;
            this.notificationEnabledCallback = notificationEnabledCallback;
        }

        public ItemViewModel Item { get; }

        readonly Action<ExtendedItemViewModel> notificationEnabledCallback;

        bool isNotificationEnabled;
        public bool IsNotificationEnabled
        {
            get => isNotificationEnabled;
            set { if (Set(ref isNotificationEnabled, value)) notificationEnabledCallback(this); }
        }
    }
}
