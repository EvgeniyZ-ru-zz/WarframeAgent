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
            model.VoidTraderNotificationChanged += ChangeEvent;
            model.VoidTraderNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var trader in model.GetCurrentVoidTrades())
            {
                var traderVM = new VoidTradeViewModel(trader);

                if (trader.Manifest != null)
                {
                    foreach (var manifest in trader.Manifest)
                    {
                        var manifestVM = new VoidItemViewModel(manifest);
                        GameView.AddVoidTradeItem(manifestVM);
                    }
                }

                GameView.AddVoidTrade(traderVM);
            }
        }

        private async void AddEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Новый торговец [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);

            if (e.Notification.Manifest != null)
            {
                foreach (var manifest in e.Notification.Manifest)
                {
                    var manifestVM = new VoidItemViewModel(manifest);
                    GameView.AddVoidTradeItem(manifestVM);
                }
            }

            var traderVM = new VoidTradeViewModel(e.Notification);
            GameView.AddVoidTrade(traderVM);
        }

        private async void ChangeEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Debug, $"Изменяю торговца [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);

            if (e.Notification.Manifest == null)
            {
                GameView.RemoveAllVoidTraderItems();
            }
            else
            {
                foreach (var manifest in e.Notification.Manifest)
                {
                    var manifestVM = new VoidItemViewModel(manifest);
                    GameView.AddVoidTradeItem(manifestVM);
                }
            }
        }

        private async void RemoveEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Удаляю торговца [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);

            GameView.RemoveVoidTradeById(e.Notification.Id);
        }
    }
}
