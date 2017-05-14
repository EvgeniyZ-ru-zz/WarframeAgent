using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Core.GameData;

namespace Agent.Model
{
    #region EventArgs

    public class NewAlertNotificationEventArgs : EventArgs
    {
        public readonly Alert Notification;

        public NewAlertNotificationEventArgs(Alert ntf)
        {
            Notification = ntf;
        }
    }

    public class RemovedAlertNotificationEventArgs : EventArgs
    {
        public readonly Alert Notification;

        public RemovedAlertNotificationEventArgs(Alert ntf)
        {
            Notification = ntf;
        }
    }

    #endregion

    #region Watcher Class

    /// <summary>
    ///     Класс слежения за обновлением списка.
    /// </summary>
    public class NotificationWatcher
    {
        private CancellationTokenSource _cts;

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => Watch(_cts.Token));
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts = null;
        }

        private async void Watch(CancellationToken ct)
        {
            try
            {
                await EvaluateList(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
        }

        private readonly Dictionary<string, Alert> currentNotifications = new Dictionary<string, Alert>();

        private async Task EvaluateList(CancellationToken ct)
        {
            var newNotifications =
                MainWindow.GameData.Data.Alerts.Where(ntf => !currentNotifications.ContainsKey(ntf.Id.Oid));
            foreach (var ntf in newNotifications)
            {
                currentNotifications.Add(ntf.Id.Oid, ntf);
                FireNewNotification(ntf);
            }
            var removedNotificationIds =
                currentNotifications.Keys.Except(MainWindow.GameData.Data.Alerts.Select(ntf => ntf.Id.Oid));
            foreach (var id in removedNotificationIds.ToList())
            {
                FireRemovedNotification(currentNotifications[id]);
                currentNotifications.Remove(id);
            }
        }

        #region Эвенты

        /// <summary>
        ///     Эвент добавления новых тревог.
        /// </summary>
        public event EventHandler<NewAlertNotificationEventArgs> AlertNotificationArrived;

        private void FireNewNotification(Alert ntf)
        {
            AlertNotificationArrived?.Invoke(this, new NewAlertNotificationEventArgs(ntf));
        }

        /// <summary>
        ///     Эвент удаления старых тревог.
        /// </summary>
        public event EventHandler<RemovedAlertNotificationEventArgs> AlertNotificationDeparted;

        private void FireRemovedNotification(Alert ntf)
        {
            AlertNotificationDeparted?.Invoke(this, new RemovedAlertNotificationEventArgs(ntf));
        }

        #endregion
    }

    #endregion


    internal class NotificationVm
    {
        public string Text { get; }

        public NotificationVm(Alert ntf)
        {
            Text = ntf.MissionInfo.Location;
        }
    }

    internal class NotificationListVm
    {
        public ObservableCollection<NotificationVm> Notifications { get; } =
            new ObservableCollection<NotificationVm>();

        public NotificationListVm()
        {
            MainWindow.NotificationWatcherwatcher.AlertNotificationArrived += OnAlertNotificationArrived;
            MainWindow.NotificationWatcherwatcher.AlertNotificationDeparted += OnAlertNotificationDeparted;
            MainWindow.NotificationWatcherwatcher.Start();
        }

        private void OnAlertNotificationArrived(object sender, NewAlertNotificationEventArgs e)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                var ntfVm = new NotificationVm(e.Notification);
                Notifications.Add(ntfVm);

                Debug.WriteLine(ntfVm.Text, "Show notification");
            });
        }

        private void OnAlertNotificationDeparted(object sender, RemovedAlertNotificationEventArgs e)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                var ntfVm = new NotificationVm(e.Notification);
                Notifications.Remove(ntfVm);

                Debug.WriteLine(ntfVm.Text, "Hide notification");
            });
        }
    }
}
