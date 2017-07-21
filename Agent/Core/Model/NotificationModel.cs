using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Model
{
    #region EventArgs

    // Добавление новых тревог
    public class NewAlertNotificationEventArgs : EventArgs
    {
        public readonly Alert Notification;

        public NewAlertNotificationEventArgs(Alert ntf)
        {
            Notification = ntf;
        }
    }

    // Удаление старых тревог
    public class RemovedAlertNotificationEventArgs : EventArgs
    {
        public readonly Alert Notification;

        public RemovedAlertNotificationEventArgs(Alert ntf)
        {
            Notification = ntf;
        }
    }

    // Добавление новых тревог
    public class NewInvasionNotificationEventArgs : EventArgs
    {
        public readonly Invasion Notification;

        public NewInvasionNotificationEventArgs(Invasion ntf)
        {
            Notification = ntf;
        }
    }

    // Удаление старых тревог
    public class RemovedInvasionNotificationEventArgs : EventArgs
    {
        public readonly Invasion Notification;

        public RemovedInvasionNotificationEventArgs(Invasion ntf)
        {
            Notification = ntf;
        }
    }

    #endregion

    #region Watcher Class

    /// <summary>
    ///     Класс слежения за обновлением списка.
    /// </summary>
    public class NotificationModel
    {
        private readonly Dictionary<string, Alert> _currentAlertsNotifications = new Dictionary<string, Alert>();
        private readonly Dictionary<string, Invasion> _currentInvasionsNotifications = new Dictionary<string, Invasion>();
        private CancellationTokenSource _cts;

        public void Start(Game game)
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => Watch(_cts.Token, game));
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts = null;
        }

        private async void Watch(CancellationToken ct, Game game)
        {
            try
            {
                await AlertEvaluateList(game); // Тревоги.
                await InvasionEvaluateList(game); // Вторжения.
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
        }

        private async Task AlertEvaluateList(Game game)
        {
            var newNotifications = game.Data.Alerts.Where(ntf => !_currentAlertsNotifications.ContainsKey(ntf.Id.Oid));
            foreach (var ntf in newNotifications)
            {
                _currentAlertsNotifications.Add(ntf.Id.Oid, ntf);
                FireNewAlertNotification(ntf);
            }
            var removedId = _currentAlertsNotifications.Keys.Except(game.Data.Alerts.Select(ntf => ntf.Id.Oid));
            foreach (var id in removedId.ToList())
            {
                FireRemovedAlertNotification(_currentAlertsNotifications[id]);
                _currentAlertsNotifications.Remove(id);
            }
        }

        private async Task InvasionEvaluateList(Game game)
        {
            var newNotifications =
                game.Data.Invasions.Where(ntf => !_currentInvasionsNotifications.ContainsKey(ntf.Id.Oid));
            foreach (var ntf in newNotifications)
            {
                _currentInvasionsNotifications.Add(ntf.Id.Oid, ntf);
                FireNewInvasionNotification(ntf);
            }
            var removedId = _currentInvasionsNotifications.Keys.Except(game.Data.Invasions.Select(ntf => ntf.Id.Oid));
            foreach (var id in removedId.ToList())
            {
                FireRemovedInvasionNotification(_currentInvasionsNotifications[id]);
                _currentInvasionsNotifications.Remove(id);
            }
        }

        #region Эвенты

        /// <summary>
        ///     Эвент добавления новых тревог.
        /// </summary>
        public event EventHandler<NewAlertNotificationEventArgs> AlertNotificationArrived;

        private void FireNewAlertNotification(Alert ntf)
        {
            AlertNotificationArrived?.Invoke(this, new NewAlertNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент удаления старых тревог.
        /// </summary>
        public event EventHandler<RemovedAlertNotificationEventArgs> AlertNotificationDeparted;

        private void FireRemovedAlertNotification(Alert ntf)
        {
            AlertNotificationDeparted?.Invoke(this, new RemovedAlertNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент добавления новых вторжений.
        /// </summary>
        public event EventHandler<NewInvasionNotificationEventArgs> InvasionNotificationArrived;

        private void FireNewInvasionNotification(Invasion ntf)
        {
            InvasionNotificationArrived?.Invoke(this, new NewInvasionNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент удаления старых вторжений.
        /// </summary>
        public event EventHandler<RemovedInvasionNotificationEventArgs> InvasionNotificationDeparted;

        private void FireRemovedInvasionNotification(Invasion ntf)
        {
            InvasionNotificationDeparted?.Invoke(this, new RemovedInvasionNotificationEventArgs(ntf));
        }

        #endregion
    }

    #endregion
}