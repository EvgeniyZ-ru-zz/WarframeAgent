using System.Collections.Generic;
using System.Linq;

using Core;
using Core.Model;
using Core.ViewModel;
using Core.Events;

using NLog;

namespace Agent.ViewModel
{
    class ItemsEngine : GenericEngineWithUpdates<ItemViewModel, Core.Model.Filter.Item>
    {
        public ItemsEngine(FiltersEvent filtersEvent) : base(filtersEvent) { }
    
        protected override ItemViewModel CreateItem(Core.Model.Filter.Item item, FiltersEvent evt) => new ItemViewModel(item);
        protected override IEnumerable<Core.Model.Filter.Item> GetItemsFromModel(GameModel model) => model.GetCurrentItems();
    
        protected override void Subscribe(GameModel model)
        {
            model.ItemNotificationArrived += AddEvent;
            model.ItemNotificationChanged += ChangeEvent;
            model.ItemNotificationDeparted += RemoveEvent;
        }
    
        protected override void LogAdded(Core.Model.Filter.Item item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новый предмет {item.Id}!");
        protected override void LogChanged(Core.Model.Filter.Item item) =>
            Tools.Logging.Send(LogLevel.Info, $"Изменённый предмет {item.Id}!");
        protected override void LogRemoved(Core.Model.Filter.Item item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю предмет {item.Id}!");
    
        protected override ItemViewModel TryGetItemByModel(Core.Model.Filter.Item item) =>
            Items.FirstOrDefault(i => i.Item.Id == item.Id);
    }
}
