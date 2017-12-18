using Core;
using Core.Model;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    class NewsEngine
    {
        private GameViewModel GameView;

        public NewsEngine(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(GameModel model)
        {
            model.NewsNotificationArrived += AddEvent;
            model.NewsNotificationDeparted += RemoveEvent;

            foreach (var news in model.GetCurrentNews())
            {
                var newsVM = new PostViewModel(news);
                GameView.AddNews(newsVM);
            }
        }

        private async void AddEvent(object sender, NewsNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Новая новость {e.Notification.Title}!", param: e.Notification);

            var newsVM = new PostViewModel(e.Notification);
            GameView.AddNews(newsVM);
        }

        private async void RemoveEvent(object sender, NewsNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Удаляю новость {e.Notification.Title}!", param: e.Notification);

            GameView.RemoveNewsByTitle(e.Notification.Title);
        }
    }
}
