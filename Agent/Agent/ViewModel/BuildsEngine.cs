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

        protected override string LogAddedOne(Build item) => $"Новое строение {item.Number}";
        protected override string LogChangedOne(Build item) => $"Изменённое строение {item.Number}";
        protected override string LogRemovedOne(Build item) => $"Удаляю строение {item.Number}";
        protected override string LogAddedMany(int n) => $"Новые строения ({n} шт.)";
        protected override string LogChangedMany(int n) => $"Изменённые строения ({n} шт.)";
        protected override string LogRemovedMany(int n) => $"Удаляю строения ({n} шт.)";

        protected override BuildViewModel TryGetItemByModel(Build item) => Items.FirstOrDefault(i => i.Id == item.Number);
    }
}
