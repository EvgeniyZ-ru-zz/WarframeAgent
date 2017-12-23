using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;

using NLog;
using System.Collections.Generic;

namespace Agent.ViewModel
{
    class DailyDealsEngine : GenericEngineWithUpdates<DailyDealViewModel, DailyDeal>
    {
        public DailyDealsEngine(FiltersEvent filtersEvent) : base(filtersEvent) { }

        protected override DailyDealViewModel CreateItem(DailyDeal item, FiltersEvent evt) => new DailyDealViewModel(item, evt);
        protected override IEnumerable<DailyDeal> GetItemsFromModel(GameModel model) => model.GetCurrentDailyDeals();

        protected override void Subscribe(GameModel model)
        {
            model.DailyDealNotificationArrived += AddEvent;
            model.DailyDealNotificationChanged += ChangeEvent;
            model.DailyDealNotificationDeparted += RemoveEvent;
        }

        protected override void LogAdded(DailyDeal item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новое предложение Дарво {item.StoreItem}!");
        protected override void LogChanged(DailyDeal item) =>
            Tools.Logging.Send(LogLevel.Debug, $"Изменённое предложение Дарво {item.StoreItem}!");
        protected override void LogRemoved(DailyDeal item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю предложение Дарво {item.StoreItem}!");

        protected override DailyDealViewModel TryGetItemByModel(DailyDeal item) => Items.FirstOrDefault(i => i.StoreItemOriginal == item.StoreItem);
    }
}
