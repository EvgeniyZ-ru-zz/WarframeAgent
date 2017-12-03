using Core;
using Core.Model;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    class BuildsEngine
    {
        private GameViewModel GameView;

        public BuildsEngine(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(GameModel model)
        {
            model.BuildNotificationArrived += AddEvent;
            model.BuildNotificationChanged += ChangeEvent;
            model.BuildNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var build in model.GetCurrentBuilds())
            {
                var buildVM = new BuildViewModel(build);
                GameView.AddBuild(buildVM);
            }
        }

        private async void AddEvent(object sender, BuildNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Новое строение {e.Notification.Number}", param: e.Notification);
            
            var buildVM = new BuildViewModel(e.Notification);
            GameView.AddBuild(buildVM);
        }

        private async void ChangeEvent(object sender, BuildNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            Tools.Logging.Send(LogLevel.Info, $"Изменённое строение {e.Notification.Number}");

            var buildVM = GameView.TryGetBuildById(e.Notification.Number);
            buildVM?.Update();
        }

        private async void RemoveEvent(object sender, BuildNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Tools.Logging.Send(LogLevel.Info, $"Удаляю строение {e.Notification.Number}", param: e.Notification);

            GameView.RemoveBuildById(e.Notification.Number);
        }
    }
}
