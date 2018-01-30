using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Agent.ViewModel.Util;
using Core;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    class UserNotificationsEngine
    {
        BatchedObservableCollection<UserNotification> notifications = new BatchedObservableCollection<UserNotification>();
        public ObservableCollection<UserNotification> Notifications => notifications;

        HashSet<ExtendedItemViewModel> itemFilter = new HashSet<ExtendedItemViewModel>();
        HashSet<string> itemKeys = new HashSet<string>(); // таких множеств нужно создать на каждый тип подписки

        public void OnSubscriptionChanged(ExtendedItemViewModel item)
        {
            if (item.IsNotificationEnabled)
            {
                itemFilter.Add(item);
                itemKeys.Add(item.Original.Item.Id);
            }
            else
            {
                itemFilter.Remove(item);
                itemKeys.Remove(item.Original.Item.Id);
            }
        }

        private GameViewModel gameVM;

        Dictionary<object, UserNotification> sourceToNotificationMapping = new Dictionary<object, UserNotification>();

        public UserNotificationsEngine(GameViewModel gameVM)
        {
            this.gameVM = gameVM;

            gameVM.Alerts.CollectionChanged += (o, e) => OnCollectionChanged(e, gameVM.Alerts, FilterAlert, CreateNotification);
            //gameVM.Invasions.CollectionChanged += (o, e) => OnCollectionChanged(e, gameVM.Invasions, CreateNotification);
        }

        bool FilterAlert(AlertViewModel alertVM) => itemKeys.Contains(alertVM.MissionInfo.Reward.Key);

        void OnCollectionChanged<ItemVM>(
            NotifyCollectionChangedEventArgs e,
            ObservableCollection<ItemVM> allItems,
            Func<ItemVM, bool> filter,
            Func<ItemVM, UserNotification> createNotification)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;

            IEnumerable<ItemVM> removedItems;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                removedItems = sourceToNotificationMapping.Keys.OfType<ItemVM>();
            else
                removedItems = e.OldItems?.Cast<ItemVM>();

            if (removedItems != null)
                RemoveAllItems(removedItems);

            IEnumerable<ItemVM> addedItems;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                addedItems = allItems;
            else
                addedItems = e.NewItems?.Cast<ItemVM>();

            var actuallyAdded = new List<ItemVM>();
            if (addedItems != null)
            {
                Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: добавляю группу");
                var notificationsToAdd = new List<UserNotification>();
                foreach (var i in addedItems)
                {
                    if (!filter(i))
                        continue;
                    if (sourceToNotificationMapping.ContainsKey(i))
                        continue;
                    actuallyAdded.Add(i);
                    var notification = createNotification(i);
                    sourceToNotificationMapping.Add(i, notification);
                    Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: добавляю нотификацию \"{notification}\"!");
                    notificationsToAdd.Add(notification);
                }
                Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: группа добавлена");
                if (notificationsToAdd.Count > 0)
                    notifications.AddRange(notificationsToAdd);
            }

            if (actuallyAdded.Count > 0)
                RemoveLater(actuallyAdded);
        }

        void RemoveAllItems<ItemVM>(IEnumerable<ItemVM> items)
        {
            Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: удаляю группу");
            var notificationsToRemove = new List<UserNotification>();
            foreach (var i in items)
            {
                if (!sourceToNotificationMapping.TryGetValue(i, out var notification))
                    continue;
                sourceToNotificationMapping.Remove(i);
                notificationsToRemove.Add(notification);
                Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: удаляю нотификацию \"{notification}\"!");
            }
            Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: группа удалена");
            notifications.RemoveAll(notificationsToRemove);
        }

        async void RemoveLater<ItemVM>(List<ItemVM> items)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            RemoveAllItems(items);
        }

        UserNotification CreateNotification(AlertViewModel i)
        {
            return new AlertUserNotification(i);
        }
    }
}
