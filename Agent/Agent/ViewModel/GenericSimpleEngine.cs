using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;
using Agent.ViewModel.Util;
using NLog;

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
            var vms = GetItemsFromModel(model).ToList();
            if (vms.Count > 0)
                AddEventImpl(vms);
        }

        protected abstract string LogAddedOne(ItemModel item);
        protected abstract string LogRemovedOne(ItemModel item);
        protected abstract string LogAddedMany(int n);
        protected abstract string LogRemovedMany(int n);

        protected virtual void LogAdded(IReadOnlyCollection<ItemModel> newItems)
        {
            var message = (newItems.Count == 1) ? LogAddedOne(newItems.First()) : LogAddedMany(newItems.Count);
            Tools.Logging.Send(LogLevel.Info, message);
        }

        protected virtual void LogRemoved(IReadOnlyCollection<ItemModel> removedItems)
        {
            var message = (removedItems.Count == 1) ? LogRemovedOne(removedItems.First()) : LogRemovedMany(removedItems.Count);
            Tools.Logging.Send(LogLevel.Info, message);
        }

        protected async void AddEvent(object sender, NotificationEventArgs<ItemModel> e)
        {
            await AsyncHelpers.RedirectToMainThread();
            AddEventImpl(e.Notifications);
        }

        protected virtual void AddEventImpl(IReadOnlyCollection<ItemModel> newItems)
        {
            LogAdded(newItems);            
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
            LogRemoved(removedItems);
            items.RemoveAll(removedItems.Select(TryGetItemByModel).Where(item => items != null));
        }
    }
}
