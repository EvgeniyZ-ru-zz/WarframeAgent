using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;
using Agent.ViewModel.Util;

namespace Agent.ViewModel
{
    abstract class GenericSimpleEngine<ItemVM, ItemModel> where ItemVM : VM
    {
        BatchedObservableCollection<ItemVM> items = new BatchedObservableCollection<ItemVM>();
        public ObservableCollection<ItemVM> Items => items;

        private FiltersEvent FiltersEvent;

        public GenericSimpleEngine(FiltersEvent filtersEvent)
        {
            FiltersEvent = filtersEvent;
        }

        protected abstract ItemVM CreateItem(ItemModel item, FiltersEvent evt);
        protected abstract IEnumerable<ItemModel> GetItemsFromModel(GameModel model);
        protected abstract void Subscribe(GameModel model);

        public void Run(GameModel model)
        {
            Subscribe(model);
            // TODO: race condition with arriving events; check if event is already there
            var vms = GetItemsFromModel(model).Select(item => CreateItem(item, FiltersEvent)).ToList();
            items.AddRange(vms);
        }

        protected abstract void LogAdded(ItemModel item);
        protected abstract void LogRemoved(ItemModel item);

        protected async void AddEvent(object sender, NotificationEventArgs<ItemModel> e)
        {
            await AsyncHelpers.RedirectToMainThread();
            AddEventImpl(e.Notification);
        }

        protected virtual void AddEventImpl(ItemModel item)
        {
            LogAdded(item);
            
            var itemVM = CreateItem(item, FiltersEvent);
            Items.Add(itemVM);
        }

        protected abstract ItemVM TryGetItemByModel(ItemModel item);

        protected async void RemoveEvent(object sender, NotificationEventArgs<ItemModel> e)
        {
            await AsyncHelpers.RedirectToMainThread();
            RemoveEventImpl(e.Notification);
        }

        protected virtual void RemoveEventImpl(ItemModel item)
        {
            LogRemoved(item);
            var itemVM = TryGetItemByModel(item);
            if (itemVM != null)
                Items.Remove(itemVM);
        }
    }
}
