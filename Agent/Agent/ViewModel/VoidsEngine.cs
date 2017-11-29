using Core;
using Core.Model;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    class VoidsEngine
    {
        private GameViewModel GameView;

        public VoidsEngine(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(GameModel model)
        {
            model.VoidTraderNotificationArrived += AddEvent;
            model.VoidTraderNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var trader in model.GetCurrentVoidTrades())
            {
                var traderVM = new VoidTradeViewModel(trader);
                GameView.AddVoidTrade(traderVM);
            }
        }

        private async void AddEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Новый торговец [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);
            
            var traderVM = new VoidTradeViewModel(e.Notification);
            GameView.AddVoidTrade(traderVM);
        }

        private async void RemoveEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Удаляю торговца [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);

            GameView.RemoveVoidTradeById(e.Notification.Id);
        }
    }
}
