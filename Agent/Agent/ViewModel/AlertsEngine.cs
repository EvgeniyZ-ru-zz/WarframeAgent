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

        protected override void LogAdded(Alert item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новая тревога {item.Id.Oid}!", param: item);
        protected override void LogRemoved(Alert item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю тревогу {item.Id.Oid}!", param: item);

        protected override AlertViewModel TryGetItemByModel(Alert item) => Items.FirstOrDefault(i => i.Id == item.Id);
    }
}
