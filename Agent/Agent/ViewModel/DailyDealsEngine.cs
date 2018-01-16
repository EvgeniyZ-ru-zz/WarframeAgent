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

        protected override string LogAddedOne(DailyDeal item) => $"Новое предложение Дарво {item.StoreItem}";
        protected override string LogChangedOne(DailyDeal item) => $"Изменённое предложение Дарво {item.StoreItem}";
        protected override string LogRemovedOne(DailyDeal item) => $"Удаляю предложение Дарво {item.StoreItem}";
        protected override string LogAddedMany(int n) => $"Новые предложения Дарво ({n} шт.)";
        protected override string LogChangedMany(int n) => $"Изменённые предложения Дарво ({n} шт.)";
        protected override string LogRemovedMany(int n) => $"Удаляю предложения Дарво ({n} шт.)";

        protected override DailyDealViewModel TryGetItemByModel(DailyDeal item) => Items.FirstOrDefault(i => i.StoreItemOriginal == item.StoreItem);
    }
}
