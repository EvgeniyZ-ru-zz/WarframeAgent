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
using Core.Events;

using NLog;

namespace Agent.ViewModel
{
    class AlertsEngine
    {
        private GameViewModel GameView;
        private FiltersEvent FiltersEvent;

        public AlertsEngine(GameViewModel gameView, FiltersEvent filtersEvent)
        {
            GameView = gameView;
            FiltersEvent = filtersEvent;
        }

        public void Run(GameModel model)
        {
            model.AlertNotificationArrived += AddEvent;
            model.AlertNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var alert in model.GetCurrentAlerts())
            {
                var alertVM = new AlertViewModel(alert, FiltersEvent);
                GameView.AddAlert(alertVM);
            }
        }

        private async void AddEvent(object sender, AlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Новая тревога {e.Notification.Id.Oid}!", param: e.Notification);
            
            var alertVM = new AlertViewModel(e.Notification, FiltersEvent);
            GameView.AddAlert(alertVM);
        }

        private async void RemoveEvent(object sender, AlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Удаляю тревогу {e.Notification.Id.Oid}!", param: e.Notification);

            GameView.RemoveAlertById(e.Notification.Id);
        }
    }
}
