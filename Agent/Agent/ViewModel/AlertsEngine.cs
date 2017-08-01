using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class AlertsEngine
    {
        private GameViewModel GameView;

        public AlertsEngine(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(GameModel model)
        {
            model.AlertNotificationArrived += AddEvent;
            model.AlertNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var alert in model.GetCurrentAlerts())
            {
                var alertVM = PrepareAlert(alert);
                GameView.AddAlert(alertVM);
            }
        }

        private async void AddEvent(object sender, NewAlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Новая тревога {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            var alertVM = PrepareAlert(e.Notification);
            GameView.AddAlert(alertVM);
        }

        AlertViewModel PrepareAlert(Alert alert)
        {
            var activation = Core.Tools.Time.ToDateTime(alert.Activation.Date.NumberLong);
            var expiry = Core.Tools.Time.ToDateTime(alert.Expiry.Date.NumberLong);
            var mission = new MissionViewModel(alert.MissionInfo);
            return new AlertViewModel(alert.Id, activation, expiry, mission);
        }

        private async void RemoveEvent(object sender, RemovedAlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Debug.WriteLine($"Удаляю тревогу {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            GameView.RemoveAlertById(e.Notification.Id);
        }
    }
}
