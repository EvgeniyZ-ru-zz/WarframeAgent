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
            model.InvasionNotificationChanged += ChangeEvent;
            model.InvasionNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var invasion in model.GetCurrentInvasions())
            {
                var invasionVM = new InvasionViewModel(invasion);
                GameView.AddInvasion(invasionVM);
            }
        }

        private async void AddEvent(object sender, InvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Новое вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            var invasionVM = new InvasionViewModel(e.Notification);
            GameView.AddInvasion(invasionVM);
        }

        private async void ChangeEvent(object sender, InvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Изменённое вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            var invasionVM = GameView.TryGetInvasionById(e.Notification.Id);
            if (invasionVM == null)
                return;
            invasionVM.Update();
        }

        private async void RemoveEvent(object sender, InvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Debug.WriteLine($"Удаляю вторжение {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            GameView.RemoveInvasionById(e.Notification.Id);
        }
    }
}
