using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;

using NLog;

namespace Agent.ViewModel
{
    class BuildsEngine : GenericEngineWithUpdates<BuildViewModel, Build>
    {
        public BuildsEngine() : base(null) { }

        protected override BuildViewModel CreateItem(Build item, FiltersEvent evt) => new BuildViewModel(item);
        protected override IEnumerable<Build> GetItemsFromModel(GameModel model) => model.GetCurrentBuilds();

        protected override void Subscribe(GameModel model)
        {
            model.BuildNotificationArrived += AddEvent;
            model.BuildNotificationChanged += ChangeEvent;
            model.BuildNotificationDeparted += RemoveEvent;
        }

        protected override void LogAdded(Build item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новое строение {item.Number}", param: item);
        protected override void LogChanged(Build item) =>
            Tools.Logging.Send(LogLevel.Debug, $"Изменённое строение {item.Number}");
        protected override void LogRemoved(Build item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю строение {item.Number}", param: item);

        protected override BuildViewModel TryGetItemByModel(Build item) => Items.FirstOrDefault(i => i.Id == item.Number);
    }
}
