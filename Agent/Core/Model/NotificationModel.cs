using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Core.Model;

namespace Core.Model
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
    public class NotificationModel
    {
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
                await AlertEvaluateList(game);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
        }

        private readonly Dictionary<string, Alert> _currentAlertsNotifications = new Dictionary<string, Alert>();

        private async Task AlertEvaluateList(Game game)
        {
            var newNotifications = game.Data.Alerts.Where(ntf => !_currentAlertsNotifications.ContainsKey(ntf.Id.Oid));
            foreach (var ntf in newNotifications)
            {
                _currentAlertsNotifications.Add(ntf.Id.Oid, ntf);
                FireNewNotification(ntf);
            }
            var removedNotificationIds =
                _currentAlertsNotifications.Keys.Except(game.Data.Alerts.Select(ntf => ntf.Id.Oid));
            foreach (var id in removedNotificationIds.ToList())
            {
                FireRemovedNotification(_currentAlertsNotifications[id]);
                _currentAlertsNotifications.Remove(id);
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

    //public class NotificationListVm
    //{
    //    public ObservableCollection<NotificationVm> Notifications { get; } =
    //        new ObservableCollection<NotificationVm>();

    //    NotificationModel model = new NotificationModel();

    //    public NotificationListVm()
    //    {
    //        model.AlertNotificationArrived += OnAlertNotificationArrived;
    //        model.AlertNotificationDeparted += OnAlertNotificationDeparted;
    //        model.Start();
    //    }

    //    private void OnAlertNotificationArrived(object sender, NewAlertNotificationEventArgs e)
    //    {
    //        System.Windows.Application.Current.Dispatcher?.Dispatcher.InvokeAsync(() =>
    //        {
    //            var ntfVm = new NotificationVm(e.Notification);
    //            Notifications.Add(ntfVm);

    //            Debug.WriteLine(ntfVm.Text, "Show notification");
    //        });
    //    }

    //    private void OnAlertNotificationDeparted(object sender, RemovedAlertNotificationEventArgs e)
    //    {
    //        MediaTypeNames.Application.Current?.Dispatcher.InvokeAsync(() =>
    //        {
    //            var ntfVm = new NotificationVm(e.Notification);
    //            Notifications.Remove(ntfVm);

    //            Debug.WriteLine(ntfVm.Text, "Hide notification");
    //        });
    //    }
    //}
}
