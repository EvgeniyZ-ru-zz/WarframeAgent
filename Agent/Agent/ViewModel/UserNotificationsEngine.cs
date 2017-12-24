using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Core;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    class UserNotificationsEngine
    {
        public ObservableCollection<UserNotification> Notifications { get; } = new ObservableCollection<UserNotification>();

        private GameViewModel gameVM;

        Dictionary<object, UserNotification> mapping = new Dictionary<object, UserNotification>();

        public UserNotificationsEngine(GameViewModel gameVM)
        {
            this.gameVM = gameVM;

            gameVM.Alerts.CollectionChanged += (o, e) => OnCollectionChanged(e, gameVM.Alerts, CreateNotification);
            //gameVM.Invasions.CollectionChanged += (o, e) => OnCollectionChanged(e, gameVM.Invasions, CreateNotification);
        }

        void OnCollectionChanged<ItemVM>(
            NotifyCollectionChangedEventArgs e,
            ObservableCollection<ItemVM> allItems,
            Func<ItemVM, UserNotification> createNotification)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;

            IEnumerable<ItemVM> removedItems;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                removedItems = mapping.Keys.OfType<ItemVM>();
            else
                removedItems = e.OldItems?.Cast<ItemVM>();

            if (removedItems != null)
                foreach (var i in removedItems)
                {
                    if (!mapping.TryGetValue(i, out var notification))
                        continue;
                    mapping.Remove(i);
                    Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: удаляю нотификацию \"{notification}\"!");
                    Notifications.Remove(notification);
                }

            IEnumerable<ItemVM> addedItems;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                addedItems = allItems;
            else
                addedItems = e.NewItems?.Cast<ItemVM>();

            List<ItemVM> actuallyAdded = new List<ItemVM>();
            if (addedItems != null)
                foreach (var i in addedItems)
                {
                    if (mapping.ContainsKey(i))
                        continue;
                    actuallyAdded.Add(i);
                    var notification = createNotification(i);
                    mapping.Add(i, notification);
                    Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: добавляю нотификацию \"{notification}\"!");
                    Notifications.Add(notification);
                }

            if (actuallyAdded.Count > 0)
                RemoveLater(actuallyAdded);
        }

        async void RemoveLater<ItemVM>(List<ItemVM> items)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            foreach (var i in items)
            {
                if (!mapping.TryGetValue(i, out var notification))
                    continue;
                mapping.Remove(i);
                Tools.Logging.Send(LogLevel.Info, $"Управление нотификациями: удаляю нотификацию \"{notification}\"!");
                Notifications.Remove(notification);
            }
        }

        UserNotification CreateNotification(AlertViewModel i)
        {
            return new AlertUserNotification(i);
        }
    }
}
