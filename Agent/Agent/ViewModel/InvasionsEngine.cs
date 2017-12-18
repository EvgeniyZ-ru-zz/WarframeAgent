using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;

using NLog;

namespace Agent.ViewModel
{
    class InvasionsEngine
    {
        private GameViewModel GameView;
        private FiltersEvent FiltersEvent;

        public InvasionsEngine(GameViewModel gameView, FiltersEvent filtersEvent)
        {
            GameView = gameView;
            FiltersEvent = filtersEvent;
        }

        public void Run(GameModel model)
        {
            model.InvasionNotificationArrived += AddEvent;
            model.InvasionNotificationChanged += ChangeEvent;
            model.InvasionNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var invasion in model.GetCurrentInvasions())
            {
                var invasionVM = new InvasionViewModel(invasion, FiltersEvent);
                GameView.AddInvasion(invasionVM);
            }
        }

        private async void AddEvent(object sender, InvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            if (!e.Notification.Completed)
            {
                Tools.Logging.Send(LogLevel.Info, $"Новое вторжение {e.Notification.Id.Oid}!");

                var invasionVM = new InvasionViewModel(e.Notification, FiltersEvent);
                GameView.AddInvasion(invasionVM);
            }
            else
            {
                Tools.Logging.Send(LogLevel.Debug, $"Вторжение {e.Notification.Id.Oid} завершено, пропускаю");
            }
        }

        private async void ChangeEvent(object sender, InvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            
            if (e.Notification.Completed)
            {
                RemoveEvent(sender, e);
            }
            else
            {
                Tools.Logging.Send(LogLevel.Info, $"Изменённое вторжение {e.Notification.Id.Oid}!");

                var invasionVM = GameView.TryGetInvasionById(e.Notification.Id);
                invasionVM?.Update();
            }
        }

        private async void RemoveEvent(object sender, InvasionNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Удаляю вторжение {e.Notification.Id.Oid}!");

            GameView.RemoveInvasionById(e.Notification.Id);
        }
    }
}
