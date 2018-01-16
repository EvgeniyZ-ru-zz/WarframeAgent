using System;
using System.Collections.Generic;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;

using NLog;
using Core.Model.Filter;

namespace Agent.ViewModel
{
    class ItemsEngine : GenericEngineWithUpdates<ItemGroupViewModel, Core.Model.Filter.Item>
    {
        public ItemsEngine(UserNotificationsEngine notificationEngine, FiltersEvent filtersEvent) : base(filtersEvent) =>
            OnSubscriptionChanged = notificationEngine.OnSubscriptionChanged;

        private Action<ExtendedItemViewModel> OnSubscriptionChanged;

        protected override ItemGroupViewModel CreateItem(Core.Model.Filter.Item item, FiltersEvent evt) => throw new NotSupportedException();
        protected override IEnumerable<Core.Model.Filter.Item> GetItemsFromModel(GameModel model) => model.GetCurrentItems();
    
        protected override void Subscribe(GameModel model)
        {
            model.ItemNotificationArrived += AddEvent;
            model.ItemNotificationChanged += ChangeEvent;
            model.ItemNotificationDeparted += RemoveEvent;
        }

        Dictionary<string, ItemGroupViewModel> groupVMs = new Dictionary<string, ItemGroupViewModel>();

        protected override void AddEventImpl(IReadOnlyCollection<Core.Model.Filter.Item> newItems)
        {
            foreach (var item in newItems)
                LogAdded(item);

            foreach (var group in newItems.Select(CreateItemExt).GroupBy(it => it.Original.Type))
            {
                if (!groupVMs.TryGetValue(group.Key, out var itemGroup))
                {
                    itemGroup = new ItemGroupViewModel(group.Key);
                    Items.Add(itemGroup);
                    groupVMs[group.Key] = itemGroup;
                }
                itemGroup.AddRange(group);
            }
        }

        ExtendedItemViewModel CreateItemExt(Core.Model.Filter.Item item)
        {
            var itemVM = new ItemViewModel(item);
            var needNotification = false; // TODO: serialize it!
            var extVM = new ExtendedItemViewModel(itemVM, needNotification, OnSubscriptionChanged);
            return extVM;
        }

        protected override void ChangeEventImpl(IReadOnlyCollection<Item> changedItems)
        {
            foreach (var item in changedItems)
            {
                LogChanged(item);

                var itemVM = TryGetItemByModelExt(item);
                if (itemVM != null)
                {
                    var oldGroupKey = itemVM.Original.Type;
                    itemVM.Update();
                    // при обновлении могла поменяться группа
                    if (oldGroupKey != itemVM.Original.Type)
                        throw new NotImplementedException("Не реализована миграция предметов между группами");
                }
            }
        }

        protected override void RemoveEventImpl(IReadOnlyCollection<Item> removedItems)
        {
            foreach (var item in removedItems)
                LogRemoved(item);
            foreach (var group in removedItems.GroupBy(i => i.Type))
            {
                if (groupVMs.TryGetValue(group.Key, out var itemGroup))
                    itemGroup.RemoveRangeByModel(group);
            }

            // подчистка пустых групп
            foreach (var (key, groupVM) in groupVMs.ToList())
            {
                if (groupVM.Items.Count != 0)
                    continue;
                Items.Remove(groupVM);
                groupVMs.Remove(key);
            }
        }

        protected override void LogAdded(Core.Model.Filter.Item item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новый предмет {item.Id}!");
        protected override void LogChanged(Core.Model.Filter.Item item) =>
            Tools.Logging.Send(LogLevel.Info, $"Изменённый предмет {item.Id}!");
        protected override void LogRemoved(Core.Model.Filter.Item item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю предмет {item.Id}!");
    
        protected override ItemGroupViewModel TryGetItemByModel(Core.Model.Filter.Item item) =>
            throw new NotSupportedException();

        protected ExtendedItemViewModel TryGetItemByModelExt(Core.Model.Filter.Item item) =>
            groupVMs.TryGetValue(item.Type, out var itemGroup) ? itemGroup.TryGetItem(item) : null;
    }
}
