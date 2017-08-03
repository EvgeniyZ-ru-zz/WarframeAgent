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

    // Добавление/удаление тревог
    public class AlertNotificationEventArgs : EventArgs
    {
        public readonly Alert Notification;

        public AlertNotificationEventArgs(Alert ntf)
        {
            Notification = ntf;
        }
    }

    // Добавление/удаление/изменение вторжений
    public class InvasionNotificationEventArgs : EventArgs
    {
        public readonly Invasion Notification;

        public InvasionNotificationEventArgs(Invasion ntf)
        {
            Notification = ntf;
        }
    }

    #endregion

    #region Watcher Class

    /// <summary>
    ///     Класс слежения за обновлением списка.
    /// </summary>
    public class GameModel
    {
        private readonly Dictionary<string, Alert> _currentAlertsNotifications = new Dictionary<string, Alert>();
        private readonly Dictionary<string, Invasion> _currentInvasionsNotifications = new Dictionary<string, Invasion>();
        private GlobalEvents.ServerEvents _server;
        private string _gameDataPath;
        private object mutex = new object();

        public void Start(GlobalEvents.ServerEvents server, string gameDataPath)
        {
            lock (mutex)
            {
                _server = server;
                _gameDataPath = gameDataPath;
                _server.Updated += UpdateSnapshot;
            }
            UpdateSnapshot();
        }

        public void Stop()
        {
            lock (mutex)
            {
                _server.Updated -= UpdateSnapshot;
                _gameDataPath = null;
                _server = null;
            }
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

        void UpdateSnapshot()
        {
            string path;
            lock (mutex)
                path = _gameDataPath;
            if (path == null)
                return;
            var snapshot = Load(path);
            AlertEvaluateList(snapshot);
            InvasionEvaluateList(snapshot);
        }

        /// <summary>
        ///     Загружаем JSON файл с игровыми данными.
        /// </summary>
        /// <param name="fileName">Путь до JSON файла</param>
        private GameSnapshotModel Load(string fileName)
        {
            GameSnapshotModel data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameSnapshotModel)serializer.Deserialize(file, typeof(GameSnapshotModel));
            }
            return data;
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

        #region Эвенты

        /// <summary>
        ///     Эвент добавления новых тревог.
        /// </summary>
        public event EventHandler<AlertNotificationEventArgs> AlertNotificationArrived;

        private void FireNewAlertNotification(Alert ntf)
        {
            AlertNotificationArrived?.Invoke(this, new AlertNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент удаления старых тревог.
        /// </summary>
        public event EventHandler<AlertNotificationEventArgs> AlertNotificationDeparted;

        private void FireRemovedAlertNotification(Alert ntf)
        {
            AlertNotificationDeparted?.Invoke(this, new AlertNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент добавления новых вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationArrived;

        private void FireNewInvasionNotification(Invasion ntf)
        {
            InvasionNotificationArrived?.Invoke(this, new InvasionNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент обновления известных вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationChanged;

        private void FireChangedInvasionNotification(Invasion ntf)
        {
            InvasionNotificationChanged?.Invoke(this, new InvasionNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент удаления старых вторжений.
        /// </summary>
        public event EventHandler<InvasionNotificationEventArgs> InvasionNotificationDeparted;

        private void FireRemovedInvasionNotification(Invasion ntf)
        {
            InvasionNotificationDeparted?.Invoke(this, new InvasionNotificationEventArgs(ntf));
        }

        #endregion
    }

    #endregion
}