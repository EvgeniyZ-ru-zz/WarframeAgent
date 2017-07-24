using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Agent.View;
using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class InvasionsViewModel
    {
        private GameViewModel GameView;

        public InvasionsViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(NotificationModel watcher)
        {
            watcher.InvasionNotificationArrived += AddEvent;
            watcher.InvasionNotificationDeparted += RemoveEvent;
        }

        private async void AddEvent(object sender, NewInvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Новое вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            e.Notification.AttackerMissionInfo.Faction = e.Notification.AttackerMissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.DefenderMissionInfo.Faction = e.Notification.DefenderMissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.Faction = e.Notification.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.NodeArray = e.Notification.Node.GetFilter(Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');

            GameView.AddInvasion(e.Notification);
        }

        private async void RemoveEvent(object sender, RemovedInvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Удаляю вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            GameView.Invasions.Remove(e.Notification);
        }
    }
}
