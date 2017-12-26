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
            AddEventImpl(e.Notifications);
        }

        protected virtual void AddEventImpl(IReadOnlyCollection<ItemModel> newItems)
        {
            foreach (var item in newItems)
                LogAdded(item);
            
            items.AddRange(newItems.Select(item => CreateItem(item, FiltersEvent)));
        }

        protected abstract ItemVM TryGetItemByModel(ItemModel item);

        protected async void RemoveEvent(object sender, NotificationEventArgs<ItemModel> e)
        {
            await AsyncHelpers.RedirectToMainThread();
            RemoveEventImpl(e.Notifications);
        }

        protected virtual void RemoveEventImpl(IReadOnlyCollection<ItemModel> removedItems)
        {
            var itemsToRemove = new List<ItemVM>();
            foreach (var item in removedItems)
                LogRemoved(item);
            items.RemoveAll(removedItems.Select(TryGetItemByModel).Where(item => items != null));
        }
    }
}
