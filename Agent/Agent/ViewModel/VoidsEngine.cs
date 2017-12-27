using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agent.ViewModel.Util;
using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;

using NLog;

namespace Agent.ViewModel
{
    class VoidsEngine
    {
        BatchedObservableCollection<VoidItemViewModel> items = new BatchedObservableCollection<VoidItemViewModel>();
        public ObservableCollection<VoidItemViewModel> Items => items;

        BatchedObservableCollection<VoidTradeViewModel> traders = new BatchedObservableCollection<VoidTradeViewModel>();
        public ObservableCollection<VoidTradeViewModel> Traders => traders;

        private FiltersEvent FiltersEvent;

        public VoidsEngine(FiltersEvent filtersEvent)
        {
            FiltersEvent = filtersEvent;
        }

        public void Run(GameModel model)
        {
            model.VoidTraderNotificationArrived += AddEvent;
            model.VoidTraderNotificationChanged += ChangeEvent;
            model.VoidTraderNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there

            AddEventimpl(model.GetCurrentVoidTrades().ToList());
        }

        private async void AddEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            foreach (var trader in e.Notifications)
                Tools.Logging.Send(LogLevel.Info, $"Новый торговец [{trader.Character}] {trader.Id.Oid}!", param: trader);

            AddEventimpl(e.Notifications);
        }

        void AddEventimpl(IReadOnlyCollection<VoidTrader> newItems)
        {
            var traderVMs = newItems.Select(trader => new VoidTradeViewModel(trader, FiltersEvent));
            var itemVMs = newItems.Where(trader => trader.Manifest != null)
                                  .SelectMany(trader => trader.Manifest.Select(m => new VoidItemViewModel(m, FiltersEvent)));
            items.AddRange(itemVMs);
            traders.AddRange(traderVMs);
        }

        // TODO: пересмотреть всю эту логику. необходима зависимость между item и trader
        private async void ChangeEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            foreach (var trader in e.Notifications)
            {
                Tools.Logging.Send(LogLevel.Debug, $"Изменяю торговца [{trader.Character}] {trader.Id.Oid}!", param: trader);

                if (trader.Manifest == null)
                {
                    //GameView.RemoveAllVoidTraderItems();
                    Items.Clear(); // TODO: ??? Выглядит подозрительно!
                }
                else
                {
                    // TODO: почему добавляются все? логика не ясна
                    foreach (var manifest in trader.Manifest)
                    {
                        var manifestVM = new VoidItemViewModel(manifest, FiltersEvent);
                        Items.Add(manifestVM);
                    }
                }
            }
        }

        private VoidTradeViewModel TryGetTraderById(Id id) => Traders.FirstOrDefault(i => i.Id == id);

        private async void RemoveEvent(object sender, VoidTraderNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            foreach (var trader in e.Notifications)
                Tools.Logging.Send(LogLevel.Info, $"Удаляю торговца [{trader.Character}] {trader.Id.Oid}!", param: trader);

            traders.RemoveAll(e.Notifications.Select(trader => TryGetTraderById(trader.Id)).Where(trader => trader != null));
        }
    }
}
