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
    class AlertsEngine : GenericSimpleEngine<AlertViewModel, Alert>
    {
        public AlertsEngine(FiltersEvent filtersEvent) : base(filtersEvent) { }

        protected override void Subscribe(GameModel model)
        {
            model.AlertNotificationArrived += AddEvent;
            model.AlertNotificationDeparted += RemoveEvent;
        }

        protected override IEnumerable<Alert> GetItemsFromModel(GameModel model) => model.GetCurrentAlerts();
        protected override AlertViewModel CreateItem(Alert item, FiltersEvent evt) => new AlertViewModel(item, evt);

        protected override string LogAddedOne(Alert item) => $"Новая тревога {item.Id.Oid}";
        protected override string LogRemovedOne(Alert item) => $"Удаляю тревогу {item.Id.Oid}";
        protected override string LogAddedMany(int n) => $"Новые тревоги ({n} шт.)";
        protected override string LogRemovedMany(int n) => $"Удаляю тревоги ({n} шт.)";

        protected override AlertViewModel TryGetItemByModel(Alert item) => Items.FirstOrDefault(i => i.Id == item.Id);
    }
}
