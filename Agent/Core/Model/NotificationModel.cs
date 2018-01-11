using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Core.Events;

namespace Core.Model
{
    #region EventArgs

    public class NotificationEventArgs<T> : EventArgs
    {
        public readonly IReadOnlyCollection<T> Notifications;
        public NotificationEventArgs(IEnumerable<T> ntf) => Notifications = ntf.ToList().AsReadOnly();
        public NotificationEventArgs(IList<T> ntf) => Notifications = new ReadOnlyCollection<T>(ntf);
    }

    // Добавление/удаление новостей
    public class NewsNotificationEventArgs : NotificationEventArgs<NewsPost>
    {
        public NewsNotificationEventArgs(IEnumerable<NewsPost> ntf) : base(ntf) { }
        public NewsNotificationEventArgs(IList<NewsPost> ntf) : base(ntf) { }
    }

    // Добавление/удаление тревог
    public class AlertNotificationEventArgs : NotificationEventArgs<Alert>
    {
        public AlertNotificationEventArgs(IEnumerable<Alert> ntf) : base(ntf) { }
        public AlertNotificationEventArgs(IList<Alert> ntf) : base(ntf) { }
    }

    // Добавление/удаление/изменение вторжений
    public class InvasionNotificationEventArgs : NotificationEventArgs<Invasion>
    {
        public InvasionNotificationEventArgs(IEnumerable<Invasion> ntf) : base(ntf) { }
        public InvasionNotificationEventArgs(IList<Invasion> ntf) : base(ntf) { }
    }

    // Добавление/удаление/изменение торговцев (баро)
    public class VoidTraderNotificationEventArgs : NotificationEventArgs<VoidTrader>
    {
        public VoidTraderNotificationEventArgs(IEnumerable<VoidTrader> ntf) : base(ntf) { }
        public VoidTraderNotificationEventArgs(IList<VoidTrader> ntf) : base(ntf) { }
    }

    // Добавление/удаление/изменение скидки дня (дарво)
    public class DailyDealNotificationEventArgs : NotificationEventArgs<DailyDeal>
    {
        public DailyDealNotificationEventArgs(IEnumerable<DailyDeal> ntf) : base(ntf) { }
        public DailyDealNotificationEventArgs(IList<DailyDeal> ntf) : base(ntf) { }
    }

    // Добавление/удаление/изменение строений
    public class BuildNotificationEventArgs : NotificationEventArgs<Build>
    {
        public BuildNotificationEventArgs(IEnumerable<Build> ntf) : base(ntf) { }
        public BuildNotificationEventArgs(IList<Build> ntf) : base(ntf) { }
    }

    #endregion

    #region Watcher Class

    /// <summary>
    ///     Класс слежения за обновлением списка.
    /// </summary>
    public class GameModel
    {
        private readonly Dictionary<string, NewsPost> _currentNewsNotifications = new Dictionary<string, NewsPost>();
        private readonly Dictionary<string, Alert> _currentAlertsNotifications = new Dictionary<string, Alert>();
        private readonly Dictionary<string, Invasion> _currentInvasionsNotifications = new Dictionary<string, Invasion>();
        private readonly Dictionary<string, VoidTrader> _currentVoidsNotifications = new Dictionary<string, VoidTrader>();
        private readonly Dictionary<string, DailyDeal> _currentDailyDealsNotifications = new Dictionary<string, DailyDeal>();
        private readonly List<Build> _currentBuilds = new List<Build>();

        private GlobalEvents.ServerEvents _server;
        private string _gameDataPath;
        private string _newsDataPath;
        private object mutex = new object();

        public void Start(GlobalEvents.ServerEvents server, string gameDataPath, string newsDataPath)
        {
            lock (mutex)
            {
                _server = server;
                _gameDataPath = gameDataPath;
                _newsDataPath = newsDataPath;
                _server.Updated += UpdateSnapshot;
            }
            UpdateSnapshot();
        }

        public void Stop()
        {
            lock (mutex)
            {
                _server.Updated -= UpdateSnapshot;
                _newsDataPath = null;
                _gameDataPath = null;
                _server = null;
            }
        }

        public IEnumerable<NewsPost> GetCurrentNews()
        {
            lock (mutex)
                return _currentNewsNotifications.Values;
        }

        public IEnumerable<Alert> GetCurrentAlerts()
        {
            lock (mutex)
                return _currentAlertsNotifications.Values;
        }

        public IEnumerable<Invasion> GetCurrentInvasions()
        {
            lock (mutex)
                return _currentInvasionsNotifications.Values;
        }

        public IEnumerable<VoidTrader> GetCurrentVoidTrades()
        {
            lock (mutex)
                return _currentVoidsNotifications.Values;
        }

        public IEnumerable<DailyDeal> GetCurrentDailyDeals()
        {
            lock (mutex)
                return _currentDailyDealsNotifications.Values;
        }

        public IEnumerable<Build> GetCurrentBuilds()
        {
            lock (mutex)
                return _currentBuilds.Where(b => b.Value > 0).ToList();
        }

        void UpdateSnapshot()
        {
            string gamePath;
            string newsPath;
            lock (mutex)
            {
                gamePath = _gameDataPath;
                newsPath = _newsDataPath;
            }

            if (gamePath == null || newsPath == null)
                return;
            var gameSnapshot = Load<GameSnapshotModel>(gamePath);
            var newsSnapshot = Load<NewsSnapshotModel>(newsPath);
            NewsEvaluteList(newsSnapshot);
            AlertEvaluateList(gameSnapshot);
            InvasionEvaluateList(gameSnapshot);
            VoidEvaluateList(gameSnapshot);
            DailyDealEvaluateList(gameSnapshot);
            BuildEvaluateList(gameSnapshot);
        }

        /// <summary>
        ///     Загружаем JSON файл с игровыми данными.
        /// </summary>
        /// <param name="fileName">Путь до JSON файла</param>
        private T Load<T>(string fileName)
        {
            T data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (T)serializer.Deserialize(file, typeof(T));
            }
            return data;
        }

        private void GenericEvaluteList<Item>(
            Item[] snapshotItems, Func<Item, string> keySelector, Func<Item, Item, bool> update,
            Dictionary<string, Item> currentList,
            Action<List<Item>> fireNew, Action<List<Item>> fireChanged, Action<List<Item>> fireRemoved) where Item : class
        {
            List<Item> newNotifications, removedNotifications = new List<Item>(), changedNotifications = null;
            lock (mutex)
            {
                newNotifications = snapshotItems.Where(ntf => !currentList.ContainsKey(keySelector(ntf))).ToList();
                foreach (var ntf in newNotifications)
                    currentList.Add(keySelector(ntf), ntf);
                if (update != null)
                {
                    changedNotifications = snapshotItems
                        .Select(ntf =>
                            currentList.TryGetValue(keySelector(ntf), out var existingNtf) && update(existingNtf, ntf) ? existingNtf : null)
                        .Where(ntf => ntf != null)
                        .ToList();
                }
                var removedId = currentList.Keys.Except(snapshotItems.Select(keySelector));
                foreach (var id in removedId.ToList())
                {
                    removedNotifications.Add(currentList[id]);
                    currentList.Remove(id);
                }
            }
            fireNew(newNotifications);
            fireChanged?.Invoke(changedNotifications);
            fireRemoved(removedNotifications);
        }

        private void NewsEvaluteList(NewsSnapshotModel snapshot) =>
            GenericEvaluteList(snapshot.Posts, ntf => ntf.Description, null, _currentNewsNotifications,
                               FireNewNewsNotification, null, FireRemovedNewsNotification);

        private void AlertEvaluateList(GameSnapshotModel snapshot) =>
            GenericEvaluteList(snapshot.Alerts, ntf => ntf.Id.Oid, null, _currentAlertsNotifications,
                               FireNewAlertNotification, null, FireRemovedAlertNotification);

        private void InvasionEvaluateList(GameSnapshotModel snapshot) =>
            GenericEvaluteList(snapshot.Invasions, ntf => ntf.Id.Oid, (oldn, newn) => oldn.Update(newn), _currentInvasionsNotifications,
                               FireNewInvasionNotification, FireChangedInvasionNotification, FireRemovedInvasionNotification);

        private void VoidEvaluateList(GameSnapshotModel snapshot) =>
            GenericEvaluteList(snapshot.VoidTraders, ntf => ntf.Id.Oid, (oldn, newn) => oldn.Update(newn), _currentVoidsNotifications,
                               FireNewVoidTraderNotification, FireChangedVoidTraderNotification, FireRemovedVoidTraderNotification);

        private void DailyDealEvaluateList(GameSnapshotModel snapshot) =>
            GenericEvaluteList(snapshot.DailyDeals, ntf => ntf.StoreItem, (oldn, newn) => oldn.Update(newn), _currentDailyDealsNotifications,
                               FireNewDailyDealNotification, FireChangedDailyDealNotification, FireRemovedDailyDealNotification);

        private void BuildEvaluateList(GameSnapshotModel snapshot)
        {
            List<Build> newNotifications = new List<Build>(), removedNotifications = new List<Build>(), changedNotifications = new List<Build>();
            lock (mutex)
            {
                int oldNumber = _currentBuilds.Count;
                int newNumber = snapshot.ProjectPct.Length;
                int commonNumber = Math.Min(oldNumber, newNumber);
                for (int i = 0; i < commonNumber; i++)
                {
                    var curr = _currentBuilds[i];
                    var newValue = snapshot.ProjectPct[i];
                    if (curr.Value != newValue)
                    {
                        if (curr.Value == 0)
                            newNotifications.Add(curr);
                        else if (newValue == 0)
                            removedNotifications.Add(curr);
                        else
                            changedNotifications.Add(curr);
                        curr.Value = newValue;
                    }
                }
                for (int i = oldNumber; i < newNumber; i++)
                {
                    var newValue = snapshot.ProjectPct[i];
                    var notification = new Build() { Number = i, Value = newValue };
                    if (newValue > 0)
                        newNotifications.Add(notification);
                    _currentBuilds.Add(notification);
                }
                for (int i = newNumber; i < oldNumber; i++)
                {
                    var curr = _currentBuilds[i];
                    if (curr.Value > 0)
                        removedNotifications.Add(curr);
                }
                if (newNumber < oldNumber)
                    _currentBuilds.RemoveRange(newNumber, newNumber - oldNumber);
            }
            FireNewBuildNotification(newNotifications);
            FireChangedBuildNotification(changedNotifications);
            FireRemovedBuildNotification(removedNotifications);
        }

        #region Эвенты

        /// <summary>
        ///     Эвент добавления новых новостей.
        /// </summary>
        public event EventHandler<NewsNotificationEventArgs> NewsNotificationArrived;

        private void FireNewNewsNotification(IList<NewsPost> ntf) =>
            NewsNotificationArrived?.Invoke(this, new NewsNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых новостей.
        /// </summary>
        public event EventHandler<NewsNotificationEventArgs> NewsNotificationDeparted;

        private void FireRemovedNewsNotification(IList<NewsPost> ntf) =>
            NewsNotificationDeparted?.Invoke(this, new NewsNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых тревог.
        /// </summary>
        public event EventHandler<AlertNotificationEventArgs> AlertNotificationArrived;

        private void FireNewAlertNotification(IList<Alert> ntf) =>
            AlertNotificationArrived?.Invoke(this, new AlertNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых тревог.
        /// </summary>
        public event EventHandler<AlertNotificationEventArgs> AlertNotificationDeparted;

        private void FireRemovedAlertNotification(IList<Alert> ntf) =>
            AlertNotificationDeparted?.Invoke(this, new AlertNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationArrived;

        private void FireNewInvasionNotification(IList<Invasion> ntf) =>
            InvasionNotificationArrived?.Invoke(this, new InvasionNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент обновления известных вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationChanged;

        private void FireChangedInvasionNotification(IList<Invasion> ntf) =>
            InvasionNotificationChanged?.Invoke(this, new InvasionNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationDeparted;

        private void FireRemovedInvasionNotification(IList<Invasion> ntf) =>
            InvasionNotificationDeparted?.Invoke(this, new InvasionNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых торговцев (баро).
        /// </summary>
        public event EventHandler<VoidTraderNotificationEventArgs> VoidTraderNotificationArrived;

        private void FireNewVoidTraderNotification(IList<VoidTrader> ntf) =>
            VoidTraderNotificationArrived?.Invoke(this, new VoidTraderNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент обновления известных торговцев (баро).
        /// </summary>
        public event EventHandler<VoidTraderNotificationEventArgs> VoidTraderNotificationChanged;

        private void FireChangedVoidTraderNotification(IList<VoidTrader> ntf) =>
            VoidTraderNotificationChanged?.Invoke(this, new VoidTraderNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых торговцев (баро).
        /// </summary>
        public event EventHandler<VoidTraderNotificationEventArgs> VoidTraderNotificationDeparted;

        private void FireRemovedVoidTraderNotification(IList<VoidTrader> ntf) =>
            VoidTraderNotificationDeparted?.Invoke(this, new VoidTraderNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых скидок дня (дарво).
        /// </summary>
        public event EventHandler<DailyDealNotificationEventArgs> DailyDealNotificationArrived;

        private void FireNewDailyDealNotification(IList<DailyDeal> ntf) =>
            DailyDealNotificationArrived?.Invoke(this, new DailyDealNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент обновления известных скидок дня (дарво).
        /// </summary>
        public event EventHandler<DailyDealNotificationEventArgs> DailyDealNotificationChanged;

        private void FireChangedDailyDealNotification(IList<DailyDeal> ntf) =>
            DailyDealNotificationChanged?.Invoke(this, new DailyDealNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых скидок дня (дарво).
        /// </summary>
        public event EventHandler<DailyDealNotificationEventArgs> DailyDealNotificationDeparted;

        private void FireRemovedDailyDealNotification(IList<DailyDeal> ntf) =>
            DailyDealNotificationDeparted?.Invoke(this, new DailyDealNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых строений.
        /// </summary>
        public event EventHandler<BuildNotificationEventArgs> BuildNotificationArrived;

        private void FireNewBuildNotification(IList<Build> ntf) =>
            BuildNotificationArrived?.Invoke(this, new BuildNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент обновления известных строений.
        /// </summary>
        public event EventHandler<BuildNotificationEventArgs> BuildNotificationChanged;

        private void FireChangedBuildNotification(IList<Build> ntf) =>
            BuildNotificationChanged?.Invoke(this, new BuildNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых строений.
        /// </summary>
        public event EventHandler<BuildNotificationEventArgs> BuildNotificationDeparted;

        private void FireRemovedBuildNotification(IList<Build> ntf) =>
            BuildNotificationDeparted?.Invoke(this, new BuildNotificationEventArgs(ntf));

        #endregion
    }

    #endregion
}