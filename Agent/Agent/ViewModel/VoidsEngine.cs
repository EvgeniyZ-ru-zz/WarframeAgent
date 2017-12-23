using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;

using NLog;

namespace Agent.ViewModel
{
    class VoidsEngine
    {
        public ObservableCollection<VoidItemViewModel> Items { get; } = new ObservableCollection<VoidItemViewModel>();
        public ObservableCollection<VoidTradeViewModel> Traders { get; } = new ObservableCollection<VoidTradeViewModel>();

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
                        Items.Add(manifestVM);
                    }
                }

                Traders.Add(traderVM);
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
                    Items.Add(manifestVM);
                }
            }

            var traderVM = new VoidTradeViewModel(e.Notification);
            Traders.Add(traderVM);
        }

        // TODO: пересмотреть всю эту логику. необходима зависимость между item и trader
        private async void ChangeEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Debug, $"Изменяю торговца [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);

            if (e.Notification.Manifest == null)
            {
                //GameView.RemoveAllVoidTraderItems();
                Items.Clear(); // ??? Выглядит подозрительно!
            }
            else
            {
                foreach (var manifest in e.Notification.Manifest)
                {
                    var manifestVM = new VoidItemViewModel(manifest);
                    Items.Add(manifestVM);
                }
            }
        }

        private VoidTradeViewModel TryGetTraderById(Id id) => Traders.FirstOrDefault(i => i.Id == id);

        private async void RemoveEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Удаляю торговца [{e.Notification.Character}] {e.Notification.Id.Oid}!", param: e.Notification);

            var traderVM = TryGetTraderById(e.Notification.Id);
            if (traderVM != null)
                Traders.Remove(traderVM);
        }
    }
}
