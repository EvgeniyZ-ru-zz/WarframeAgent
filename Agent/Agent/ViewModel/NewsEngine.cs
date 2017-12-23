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
    class NewsEngine : GenericSimpleEngine<PostViewModel, NewsPost>
    {
        public NewsEngine() : base(null) { }

        protected override PostViewModel CreateItem(NewsPost item, FiltersEvent evt) => new PostViewModel(item);
        protected override IEnumerable<NewsPost> GetItemsFromModel(GameModel model) => model.GetCurrentNews();

        protected override void Subscribe(GameModel model)
        {
            model.NewsNotificationArrived += AddEvent;
            model.NewsNotificationDeparted += RemoveEvent;
        }

        protected override void LogAdded(NewsPost item) =>
            Tools.Logging.Send(LogLevel.Info, $"Новая новость {item.Title}!", param: item);
        protected override void LogRemoved(NewsPost item) =>
            Tools.Logging.Send(LogLevel.Info, $"Удаляю новость {item.Title}!", param: item);

        protected override PostViewModel TryGetItemByModel(NewsPost item) => Items.FirstOrDefault(a => a.Description == item.Title);
    }
}
