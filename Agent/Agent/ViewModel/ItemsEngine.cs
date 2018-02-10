using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Model;
using Core.Model.Filter;
using Core.ViewModel;
using Core.Events;

using Agent.ViewModel.Util;

namespace Agent.ViewModel
{
    class ItemsEngine : GenericEngineWithUpdates<ItemGroupViewModel, Core.Model.Filter.Item>
    {
        public ItemsEngine(UserNotificationsEngine notificationEngine, FiltersEvent filtersEvent) : base(filtersEvent) =>
            this.notificationEngine = notificationEngine;

        BatchedObservableCollection<ItemGroupViewModel> enabledItems = new BatchedObservableCollection<ItemGroupViewModel>();
        public ObservableCollection<ItemGroupViewModel> EnabledItems => enabledItems;

        UserNotificationsEngine notificationEngine;

        protected override ItemGroupViewModel CreateItem(Core.Model.Filter.Item item, FiltersEvent evt) => throw new NotSupportedException();
        protected override IEnumerable<Core.Model.Filter.Item> GetItemsFromModel(GameModel model) => model.GetCurrentItems();
    
        protected override void Subscribe(GameModel model)
        {
            model.ItemNotificationArrived += AddEvent;
            model.ItemNotificationChanged += ChangeEvent;
            model.ItemNotificationDeparted += RemoveEvent;
        }

        Dictionary<string, ItemGroupViewModel> groupVMs = new Dictionary<string, ItemGroupViewModel>();
        Dictionary<string, ItemGroupViewModel> enabledGroupVMs = new Dictionary<string, ItemGroupViewModel>();

        static bool EnabledFilter(ExtendedItemViewModel itemVM) => itemVM.Original.Enabled;

        protected override void AddEventImpl(IReadOnlyCollection<Core.Model.Filter.Item> newItems)
        {
            LogAdded(newItems);

            foreach (var group in newItems.Select(CreateItemExt).GroupBy(it => it.Original.Type))
            {
                if (!groupVMs.TryGetValue(group.Key, out var itemGroup))
                {
                    itemGroup = new ItemGroupViewModel(group.Key);
                    Items.Add(itemGroup);
                    groupVMs[group.Key] = itemGroup;
                }
                itemGroup.AddRange(group);
                var enabledGroup = group.Where(EnabledFilter).ToList();
                if (enabledGroup.Count > 0)
                {
                    if (!enabledGroupVMs.TryGetValue(group.Key, out var enabledItemGroup))
                    {
                        enabledItemGroup = new ItemGroupViewModel(group.Key);
                        EnabledItems.Add(enabledItemGroup);
                        enabledGroupVMs[group.Key] = enabledItemGroup;
                    }
                    enabledItemGroup.AddRange(enabledGroup);
                }
            }
        }

        ExtendedItemViewModel CreateItemExt(Core.Model.Filter.Item item)
        {
            var itemVM = new ItemViewModel(item);
            var notificationState = notificationEngine.GetNotificationState(item);
            var extVM = new ExtendedItemViewModel(itemVM, notificationState);

            foreach (var (target, state) in notificationState)
                state.PropertyChanged += (o, args) => notificationEngine.OnSubscriptionChanged(extVM, target);

            return extVM;
        }

        protected override void ChangeEventImpl(IReadOnlyCollection<Item> changedItems)
        {
            LogChanged(changedItems);
            foreach (var item in changedItems)
            {
                var itemVM = TryGetItemByModelExt(item);
                if (itemVM != null)
                {
                    var oldGroupKey = itemVM.Original.Type;
                    var oldEnabledState = itemVM.Original.Enabled;
                    itemVM.Update();
                    // при обновлении могла поменяться группа
                    if (oldGroupKey != itemVM.Original.Type)
                        throw new NotImplementedException("Не реализована миграция предметов между группами");
                    // при обновлении элемент мог «включиться»
                    if (oldEnabledState != itemVM.Original.Enabled)
                        throw new NotImplementedException("Не реализована обновление состояния отключённости"); // TODO: не бросать исключение, а просто залогировать?
                }
            }
        }

        protected override void RemoveEventImpl(IReadOnlyCollection<Item> removedItems)
        {
            LogRemoved(removedItems);
            foreach (var group in removedItems.GroupBy(i => i.Type))
            {
                if (groupVMs.TryGetValue(group.Key, out var itemGroup))
                    itemGroup.RemoveRangeByModel(group);
                if (enabledGroupVMs.TryGetValue(group.Key, out var enabledItemGroup))
                    enabledItemGroup.RemoveRangeByModel(group);
            }

            // подчистка пустых групп
            foreach (var (key, groupVM) in groupVMs.ToList())
            {
                if (groupVM.Items.Count != 0)
                    continue;
                Items.Remove(groupVM);
                groupVMs.Remove(key);
            }

            foreach (var (key, enabledGroupVM) in enabledGroupVMs.ToList())
            {
                if (enabledGroupVM.Items.Count != 0)
                    continue;
                EnabledItems.Remove(enabledGroupVM);
                enabledGroupVMs.Remove(key);
            }
        }

        protected override string LogAddedOne(Core.Model.Filter.Item item) => $"Новый предмет {item.Id}!";
        protected override string LogChangedOne(Core.Model.Filter.Item item) => $"Изменённый предмет {item.Id}!";
        protected override string LogRemovedOne(Core.Model.Filter.Item item) => $"Удаляю предмет {item.Id}!";
        protected override string LogAddedMany(int n) => $"Новые предметы ({n} шт.)";
        protected override string LogChangedMany(int n) => $"Изменённые предметы ({n} шт.)";
        protected override string LogRemovedMany(int n) => $"Удаляю предметы ({n} шт.)";

        protected override ItemGroupViewModel TryGetItemByModel(Core.Model.Filter.Item item) =>
            throw new NotSupportedException();

        protected ExtendedItemViewModel TryGetItemByModelExt(Core.Model.Filter.Item item) =>
            groupVMs.TryGetValue(item.Type, out var itemGroup) ? itemGroup.TryGetItem(item) : null;
    }
}
