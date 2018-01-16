using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Core.Model;
using Core.ViewModel;
using Core.Events;

namespace Agent.ViewModel
{
    public class GameViewModel
    {
        public GameViewModel(GameModel model, FiltersEvent filtersEvent)
        {
            var reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            reloadTimer.Tick += reloadTimer_Elapsed;
            reloadTimer.Start();

            Model = model;

            NewsEngine = new NewsEngine();
            AlertsEngine = new AlertsEngine(filtersEvent);
            InvasionsEngine = new InvasionsEngine(filtersEvent);
            VoidsEngine = new VoidsEngine(filtersEvent);
            DailyDealsEngine = new DailyDealsEngine(filtersEvent);
            BuildsEngine = new BuildsEngine();
            UserNotificationsEngine = new UserNotificationsEngine(this);
            ItemsEngine = new ItemsEngine(UserNotificationsEngine, filtersEvent);
        }

        public void Run()
        {
            NewsEngine.Run(Model);
            AlertsEngine.Run(Model);
            InvasionsEngine.Run(Model);
            VoidsEngine.Run(Model);
            DailyDealsEngine.Run(Model);
            ItemsEngine.Run(Model);
            BuildsEngine.Run(Model);
        }

        private NewsEngine NewsEngine;
        private AlertsEngine AlertsEngine;
        private InvasionsEngine InvasionsEngine;
        private VoidsEngine VoidsEngine;
        private DailyDealsEngine DailyDealsEngine;
        private ItemsEngine ItemsEngine;
        private BuildsEngine BuildsEngine;
        private UserNotificationsEngine UserNotificationsEngine;
        private GameModel Model;

        public ObservableCollection<PostViewModel> News => NewsEngine.Items;
        public ObservableCollection<AlertViewModel> Alerts => AlertsEngine.Items;
        public ObservableCollection<InvasionViewModel> Invasions => InvasionsEngine.Items;
        public ObservableCollection<VoidTradeViewModel> VoidTrades => VoidsEngine.Traders;
        public ObservableCollection<VoidItemViewModel> VoidTradeItems => VoidsEngine.Items;
        public ObservableCollection<DailyDealViewModel> DailyDeals => DailyDealsEngine.Items;
        public ObservableCollection<ItemGroupViewModel> ItemGroups => ItemsEngine.Items;
        public ObservableCollection<BuildViewModel> Builds => BuildsEngine.Items;
        public ObservableCollection<UserNotification> UserNotifications => UserNotificationsEngine.Notifications;

        public EarthTimeViewModel EarthTime { get; } = new EarthTimeViewModel();
        public CetusTimeViewModel CetusTime { get; } = new CetusTimeViewModel();
        public EarthTimeViewModel EidolonTime { get; } = new EarthTimeViewModel();

        private void reloadTimer_Elapsed(object sender, EventArgs e)
        {
            for (var index = 0; index < (Alerts).Count; index++)
            {
                var item = (Alerts)[index];
                item.UpdateStatus(); // TODO: make it inside the alert
            }

            for (var index = 0; index < (VoidTrades).Count; index++)
            {
                var item = (VoidTrades)[index];
                item.UpdateStatus(); // TODO: make it inside the trade
            }

            for (var index = 0; index < (DailyDeals).Count; index++)
            {
                var item = (DailyDeals)[index];
                item.UpdateStatus(); // TODO: make it inside the dailyDeals
            }

            EarthTime.UpdateTime();
        }
    }
}