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
        public static void AddEvent(object sender, NewInvasionNotificationEventArgs e)
        {
            Debug.WriteLine($"Новое вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            e.Notification.AttackerMissionInfo.Faction = e.Notification.AttackerMissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.DefenderMissionInfo.Faction = e.Notification.DefenderMissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.Faction = e.Notification.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.NodeArray = e.Notification.Node.GetFilter(Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');

            Application.Current.Dispatcher?.InvokeAsync(() =>
            {
                if (MainWindow.GameView.Invasions == null) MainWindow.GameView.Invasions = new ObservableCollection<Invasion>();
                MainWindow.GameView.Invasions.Add(e.Notification);
            });
        }
    

        public static void RemoveEvent(object sender, RemovedInvasionNotificationEventArgs e)
        {
            Debug.WriteLine($"Удаляю вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            Application.Current.Dispatcher?.InvokeAsync(() =>
            {
                MainWindow.GameView.Invasions.Remove(e.Notification);
            });
        }
    }
}
