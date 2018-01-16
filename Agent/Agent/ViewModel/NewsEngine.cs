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

        protected override string LogAddedOne(NewsPost item) => $"Новая новость {item.Title}";
        protected override string LogRemovedOne(NewsPost item) => $"Удаляю новость {item.Title}";
        protected override string LogAddedMany(int n) => $"Новые новости ({n} шт.)";
        protected override string LogRemovedMany(int n) => $"Удаляю новости ({n} шт.)";

        protected override PostViewModel TryGetItemByModel(NewsPost item) => Items.FirstOrDefault(a => a.Description == item.Title);
    }
}
