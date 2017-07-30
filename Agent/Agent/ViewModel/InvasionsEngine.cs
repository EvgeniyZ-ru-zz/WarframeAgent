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
    class InvasionsEngine
    {
        private GameViewModel GameView;

        public InvasionsEngine(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(GameModel model)
        {
            model.InvasionNotificationArrived += AddEvent;
            model.InvasionNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var invasion in model.GetCurrentInvasions())
            {
                PrepareInvasion(invasion);
                GameView.AddInvasion(invasion);
            }
        }

        private async void AddEvent(object sender, NewInvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Новое вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            PrepareInvasion(e.Notification);

            GameView.AddInvasion(e.Notification);
        }

        void PrepareInvasion(Invasion invasion)
        {
            invasion.AttackerMissionInfo.Faction = invasion.AttackerMissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            invasion.DefenderMissionInfo.Faction = invasion.DefenderMissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            invasion.Faction = invasion.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            invasion.NodeArray = invasion.Node.GetFilter(Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
        }

        private async void RemoveEvent(object sender, RemovedInvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Удаляю вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            GameView.Invasions.Remove(e.Notification);
        }
    }
}
