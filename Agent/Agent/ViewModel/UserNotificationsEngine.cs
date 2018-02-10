using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Agent.ViewModel.Util;
using Core;
using Core.Model.Filter;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    class UserNotificationsEngine
    {
        BatchedObservableCollection<UserNotification> notifications = new BatchedObservableCollection<UserNotification>();
        public ObservableCollection<UserNotification> Notifications => notifications;

        HashSet<ExtendedItemViewModel> itemFilter = new HashSet<ExtendedItemViewModel>();
        HashSet<string> alertItemKeys = new HashSet<string>();
        HashSet<string> invasionItemKeys = new HashSet<string>();

        public void OnSubscriptionChanged(ExtendedItemViewModel item, NotificationTarget target)
        {
            bool newState = item.NotificationState[target].NotificationEnabled;
            switch (target)
            {
            case NotificationTarget.Alert:
                OnGenericSubscriptionChanged(item, alertItemKeys, newState);
                break;
            case NotificationTarget.Invasion:
                OnGenericSubscriptionChanged(item, invasionItemKeys, newState);
                break;
            default:
                throw new NotImplementedException();
            }

            UpdateSettings(item.Item.Id, target, newState);
        }

        void UpdateSettings(string id, NotificationTarget target, bool state)
        {
            var settingsById = Settings.Program.UserNotifications.ById;
            string targetString = target.ToString();
            if (state)
            {
                if (!settingsById.TryGetValue(id, out var values))
                {
                    values = new HashSet<string>();
                    settingsById[id] = values;
                }
                values.Add(targetString);
            }
            else
            {
                if (settingsById.TryGetValue(id, out var values))
                {
                    values.Remove(targetString);
                    if (values.Count == 0)
                        settingsById.Remove(id);
                }
            }
            // TODO: сделать сохранение настроек асинхронным
            // и писать не каждый раз, а время от времени
            Settings.Program.Save();
        }

        void OnGenericSubscriptionChanged(ExtendedItemViewModel item, HashSet<string> itemKeys, bool newState)
        {
            if (newState)
            {
                itemFilter.Add(item);
                itemKeys.Add(item.Item.Id);
            }
            else
            {
                itemFilter.Remove(item);
                itemKeys.Remove(item.Item.Id);
            }
        }

        private GameViewModel gameVM;

        Dictionary<object, UserNotification> sourceToNotificationMapping = new Dictionary<object, UserNotification>();

        public UserNotificationsEngine(GameViewModel gameVM)
        {
            this.gameVM = gameVM;

            gameVM.Alerts.CollectionChanged += (o, e) => OnCollectionChanged(e, gameVM.Alerts, FilterAlert, CreateNotification);
            gameVM.Invasions.CollectionChanged += (o, e) => OnCollectionChanged(e, gameVM.Invasions, FilterInvasion, CreateNotification);
        }

        bool FilterAlert(AlertViewModel alertVM) => alertItemKeys.Contains(alertVM.MissionInfo.Reward.Key);
        bool FilterInvasion(InvasionViewModel invasionVM) => invasionItemKeys.Contains(invasionVM.DefenderReward.Key) ||
                                                             invasionItemKeys.Contains(invasionVM.AttackerReward.Key);

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

        public Dictionary<NotificationTarget, SubscriptionState> GetNotificationState(string id)
        {
            var notificationState = new Dictionary<NotificationTarget, SubscriptionState>()
            {
                [NotificationTarget.Alert] = new SubscriptionState(),
                [NotificationTarget.Invasion] = new SubscriptionState()
            };
            if (Settings.Program.UserNotifications.ById.TryGetValue(id, out var watchedValues))
            {
                foreach (var value in watchedValues)
                {
                    if (Enum.TryParse<NotificationTarget>(value, out var target) && notificationState.ContainsKey(target))
                        notificationState[target].NotificationEnabled = true;
                }
            }
            return notificationState;
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

        UserNotification CreateNotification(AlertViewModel i) => new AlertUserNotification(i);
        UserNotification CreateNotification(InvasionViewModel i) => new InvasionUserNotification(i);
    }
}
