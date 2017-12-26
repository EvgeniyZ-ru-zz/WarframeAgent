using System.Collections.Generic;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;

using NLog;

namespace Agent.ViewModel
{
    class InvasionsEngine : GenericEngineWithUpdates<InvasionViewModel, Invasion>
    {
        public InvasionsEngine(FiltersEvent filtersEvent) : base(filtersEvent) { }

        protected override InvasionViewModel CreateItem(Invasion item, FiltersEvent evt) => new InvasionViewModel(item, evt);
        protected override IEnumerable<Invasion> GetItemsFromModel(GameModel model) => model.GetCurrentInvasions();

        protected override void Subscribe(GameModel model)
        {
            model.InvasionNotificationArrived += AddEvent;
            model.InvasionNotificationChanged += ChangeEvent;
            model.InvasionNotificationDeparted += RemoveEvent;
        }

        protected override void LogAdded(Invasion item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новое вторжение {item.Id.Oid}!");
        protected override void LogChanged(Invasion item) =>
            Tools.Logging.Send(LogLevel.Info, $"Изменённое вторжение {item.Id.Oid}!");
        protected override void LogRemoved(Invasion item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю вторжение {item.Id.Oid}!");

        protected override InvasionViewModel TryGetItemByModel(Invasion item) => Items.FirstOrDefault(i => i.Id == item.Id);

        protected override void AddEventImpl(IReadOnlyCollection<Invasion> newItems)
        {
            var notCompletedInvasions = newItems.Where(item => !item.Completed).ToList();
            if (notCompletedInvasions.Count > 0)
                base.AddEventImpl(notCompletedInvasions);
            foreach (var item in newItems.Where(item => item.Completed))
                Tools.Logging.Send(LogLevel.Debug, $"Вторжение {item.Id.Oid} завершено, пропускаю");
        }

        protected override void ChangeEventImpl(IReadOnlyCollection<Invasion> changedItems)
        {
            var nonCompletedInvasions = changedItems.Where(item => !item.Completed).ToList();
            if (nonCompletedInvasions.Count > 0)
                base.ChangeEventImpl(nonCompletedInvasions);
            var completedInvasions = changedItems.Where(item => item.Completed).ToList();
            if (completedInvasions.Count > 0)
                base.RemoveEventImpl(completedInvasions);
        }
    }
}
