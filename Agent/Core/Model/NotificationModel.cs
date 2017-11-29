using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Core.Events;

namespace Core.Model
{
    #region EventArgs

    // Добавление/удаление новостей
    public class NewsNotificationEventArgs : EventArgs
    {
        public readonly NewsPost Notification;
        public NewsNotificationEventArgs(NewsPost ntf) =>
            Notification = ntf;
    }

    // Добавление/удаление тревог
    public class AlertNotificationEventArgs : EventArgs
    {
        public readonly Alert Notification;
        public AlertNotificationEventArgs(Alert ntf) =>
            Notification = ntf;
    }

    // Добавление/удаление/изменение вторжений
    public class InvasionNotificationEventArgs : EventArgs
    {
        public readonly Invasion Notification;
        public InvasionNotificationEventArgs(Invasion ntf) =>
            Notification = ntf;
    }

    // Добавление/удаление/изменение торговцев
    public class VoidTraderNotificationEventArgs : EventArgs
    {
        public readonly VoidTrader Notification;
        public VoidTraderNotificationEventArgs(VoidTrader ntf) =>
            Notification = ntf;
    }

    // Добавление/удаление/изменение строений
    public class BuildNotificationEventArgs : EventArgs
    {
        public readonly Build Notification;
        public BuildNotificationEventArgs(Build ntf) =>
            Notification = ntf;
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
            VoidsEvaluateList(gameSnapshot);
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

        private void NewsEvaluteList(NewsSnapshotModel snapshot)
        {
            List<NewsPost> newNotifications, removedNotifications = new List<NewsPost>();
            lock (mutex)
            {
                newNotifications = snapshot.Posts.Where(ntf => !_currentNewsNotifications.ContainsKey(ntf.Description)).ToList();
                foreach (var ntf in newNotifications)
                    _currentNewsNotifications.Add(ntf.Description, ntf);
                var removedId = _currentNewsNotifications.Keys.Except(snapshot.Posts.Select(ntf => ntf.Description));
                foreach (var id in removedId.ToList())
                {
                    removedNotifications.Add(_currentNewsNotifications[id]);
                    _currentNewsNotifications.Remove(id);
                }
            }
            foreach (var ntf in newNotifications)
                FireNewNewsNotification(ntf);
            foreach (var ntf in removedNotifications)
                FireRemovedNewsNotification(ntf);
        }

        private void AlertEvaluateList(GameSnapshotModel snapshot)
        {
            List<Alert> newNotifications, removedNotifications = new List<Alert>();
            lock (mutex)
            {
                newNotifications = snapshot.Alerts.Where(ntf => !_currentAlertsNotifications.ContainsKey(ntf.Id.Oid)).ToList();
                foreach (var ntf in newNotifications)
                    _currentAlertsNotifications.Add(ntf.Id.Oid, ntf);
                var removedId = _currentAlertsNotifications.Keys.Except(snapshot.Alerts.Select(ntf => ntf.Id.Oid));
                foreach (var id in removedId.ToList())
                {
                    removedNotifications.Add(_currentAlertsNotifications[id]);
                    _currentAlertsNotifications.Remove(id);
                }
            }
            foreach (var ntf in newNotifications)
                FireNewAlertNotification(ntf);
            foreach (var ntf in removedNotifications)
                FireRemovedAlertNotification(ntf);
        }

        private void InvasionEvaluateList(GameSnapshotModel snapshot)
        {
            List<Invasion> newNotifications, removedNotifications = new List<Invasion>(), changedNotifications;
            lock (mutex)
            {
                newNotifications = snapshot.Invasions.Where(ntf => !_currentInvasionsNotifications.ContainsKey(ntf.Id.Oid)).ToList();
                foreach (var ntf in newNotifications)
                    _currentInvasionsNotifications.Add(ntf.Id.Oid, ntf);
                changedNotifications = snapshot.Invasions
                    .Select(ntf =>
                        _currentInvasionsNotifications.TryGetValue(ntf.Id.Oid, out var existingNtf) && existingNtf.Update(ntf) ? existingNtf : null)
                    .Where(ntf => ntf != null)
                    .ToList();
                var removedId = _currentInvasionsNotifications.Keys.Except(snapshot.Invasions.Select(ntf => ntf.Id.Oid));
                foreach (var id in removedId.ToList())
                {
                    removedNotifications.Add(_currentInvasionsNotifications[id]);
                    _currentInvasionsNotifications.Remove(id);
                }
            }
            foreach (var ntf in newNotifications)
                FireNewInvasionNotification(ntf);
            foreach (var ntf in changedNotifications)
                FireChangedInvasionNotification(ntf);
            foreach (var ntf in removedNotifications)
                FireRemovedInvasionNotification(ntf);
        }

        private void VoidsEvaluateList(GameSnapshotModel snapshot)
        {
            List<VoidTrader> newNotifications, removedNotifications = new List<VoidTrader>();
            lock (mutex)
            {
                newNotifications = snapshot.VoidTraders.Where(ntf => !_currentVoidsNotifications.ContainsKey(ntf.Id.Oid)).ToList();
                foreach (var ntf in newNotifications)
                    _currentVoidsNotifications.Add(ntf.Id.Oid, ntf);
                var removedId = _currentVoidsNotifications.Keys.Except(snapshot.VoidTraders.Select(ntf => ntf.Id.Oid));
                foreach (var id in removedId.ToList())
                {
                    removedNotifications.Add(_currentVoidsNotifications[id]);
                    _currentVoidsNotifications.Remove(id);
                }
            }
            foreach (var ntf in newNotifications)
                FireNewVoidTraderNotification(ntf);
            foreach (var ntf in removedNotifications)
                FireRemovedVoidTraderNotification(ntf);
        }

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
                    Build notification = new Build() { Number = i, Value = newValue };
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
            foreach (var ntf in newNotifications)
                FireNewBuildNotification(ntf);
            foreach (var ntf in changedNotifications)
                FireChangedBuildNotification(ntf);
            foreach (var ntf in removedNotifications)
                FireRemovedBuildNotification(ntf);
        }

        #region Эвенты

        /// <summary>
        ///     Эвент добавления новых новостей.
        /// </summary>
        public event EventHandler<NewsNotificationEventArgs> NewsNotificationArrived;

        private void FireNewNewsNotification(NewsPost ntf) =>
            NewsNotificationArrived?.Invoke(this, new NewsNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых новостей.
        /// </summary>
        public event EventHandler<NewsNotificationEventArgs> NewsNotificationDeparted;

        private void FireRemovedNewsNotification(NewsPost ntf) =>
            NewsNotificationDeparted?.Invoke(this, new NewsNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых тревог.
        /// </summary>
        public event EventHandler<AlertNotificationEventArgs> AlertNotificationArrived;

        private void FireNewAlertNotification(Alert ntf) =>
            AlertNotificationArrived?.Invoke(this, new AlertNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых тревог.
        /// </summary>
        public event EventHandler<AlertNotificationEventArgs> AlertNotificationDeparted;

        private void FireRemovedAlertNotification(Alert ntf) =>
            AlertNotificationDeparted?.Invoke(this, new AlertNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationArrived;

        private void FireNewInvasionNotification(Invasion ntf) =>
            InvasionNotificationArrived?.Invoke(this, new InvasionNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент обновления известных вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationChanged;

        private void FireChangedInvasionNotification(Invasion ntf) =>
            InvasionNotificationChanged?.Invoke(this, new InvasionNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationDeparted;

        private void FireRemovedInvasionNotification(Invasion ntf) =>
            InvasionNotificationDeparted?.Invoke(this, new InvasionNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых торговцев.
        /// </summary>
        public event EventHandler<VoidTraderNotificationEventArgs> VoidTraderNotificationArrived;

        private void FireNewVoidTraderNotification(VoidTrader ntf) =>
            VoidTraderNotificationArrived?.Invoke(this, new VoidTraderNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых торговцев.
        /// </summary>
        public event EventHandler<VoidTraderNotificationEventArgs> VoidTraderNotificationDeparted;

        private void FireRemovedVoidTraderNotification(VoidTrader ntf) =>
            VoidTraderNotificationDeparted?.Invoke(this, new VoidTraderNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент добавления новых строений.
        /// </summary>
        public event EventHandler<BuildNotificationEventArgs> BuildNotificationArrived;

        private void FireNewBuildNotification(Build ntf) =>
            BuildNotificationArrived?.Invoke(this, new BuildNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент обновления известных строений.
        /// </summary>
        public event EventHandler<BuildNotificationEventArgs> BuildNotificationChanged;

        private void FireChangedBuildNotification(Build ntf) =>
            BuildNotificationChanged?.Invoke(this, new BuildNotificationEventArgs(ntf));

        /// <summary>
        ///     Эвент удаления старых строений.
        /// </summary>
        public event EventHandler<BuildNotificationEventArgs> BuildNotificationDeparted;

        private void FireRemovedBuildNotification(Build ntf) =>
            BuildNotificationDeparted?.Invoke(this, new BuildNotificationEventArgs(ntf));

        #endregion
    }

    #endregion
}