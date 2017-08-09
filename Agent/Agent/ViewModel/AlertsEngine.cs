using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Core;
using Core.Model;
using Core.ViewModel;
using NLog;

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
                var alertVM = new AlertViewModel(alert);
                GameView.AddAlert(alertVM);
            }
        }

        private async void AddEvent(object sender, AlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Debug, $"Новая тревога {e.Notification.Id.Oid}!");
            
            var alertVM = new AlertViewModel(e.Notification);
            GameView.AddAlert(alertVM);
        }

        private async void RemoveEvent(object sender, AlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Debug, $"Удаляю тревогу {e.Notification.Id.Oid}!");

            GameView.RemoveAlertById(e.Notification.Id);
        }
    }
}
