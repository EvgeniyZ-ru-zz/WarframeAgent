using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;

using NLog;

namespace Agent.ViewModel
{
    class DailyDealsEngine
    {
        private GameViewModel GameView;
        private FiltersEvent FiltersEvent;

        public DailyDealsEngine(GameViewModel gameView, FiltersEvent filtersEvent)
        {
            GameView = gameView;
            FiltersEvent = filtersEvent;
        }

        public void Run(GameModel model)
        {
            model.DailyDealNotificationArrived += AddEvent;
            model.DailyDealNotificationChanged += ChangeEvent;
            model.DailyDealNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var deal in model.GetCurrentDailyDeals())
            {
                var dailyDealVM = new DailyDealViewModel(deal, FiltersEvent);
                GameView.AddDailyDeal(dailyDealVM);
            }
        }

        private async void AddEvent(object sender, DailyDealNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Новое предложение Дарво {e.Notification.StoreItem}!");

            var dailyDealVM = new DailyDealViewModel(e.Notification, FiltersEvent);
            GameView.AddDailyDeal(dailyDealVM);
        }

        private async void ChangeEvent(object sender, DailyDealNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Изменённое предложения Дарво {e.Notification.StoreItem}!");

            var dailyDealVM = GameView.TryGetDailyDealByName(e.Notification.StoreItem);
            dailyDealVM?.Update();
        }

        private async void RemoveEvent(object sender, DailyDealNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Удаляю предложение Дарво {e.Notification.StoreItem}!");

            GameView.RemoveDailyDealByName(e.Notification.StoreItem);
        }
    }
}
