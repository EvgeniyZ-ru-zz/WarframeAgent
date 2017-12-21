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
            NewsEngine = new NewsEngine(this);
            AlertsEngine = new AlertsEngine(this, filtersEvent);
            InvasionsEngine = new InvasionsEngine(this, filtersEvent);
            VoidsEngine = new VoidsEngine(this);
            DailyDealsEngine = new DailyDealsEngine(this, filtersEvent);
            BuildsEngine = new BuildsEngine(this);
        }

        public void Run()
        {
            NewsEngine.Run(Model);
            AlertsEngine.Run(Model);
            InvasionsEngine.Run(Model);
            VoidsEngine.Run(Model);
            DailyDealsEngine.Run(Model);
            BuildsEngine.Run(Model);
        }

        private NewsEngine NewsEngine;
        private AlertsEngine AlertsEngine;
        private InvasionsEngine InvasionsEngine;
        private VoidsEngine VoidsEngine;
        private DailyDealsEngine DailyDealsEngine;
        private BuildsEngine BuildsEngine;
        private GameModel Model;

        public ObservableCollection<PostViewModel> News { get; } = new ObservableCollection<PostViewModel>();
        public ObservableCollection<AlertViewModel> Alerts { get; } = new ObservableCollection<AlertViewModel>();
        public ObservableCollection<InvasionViewModel> Invasions { get; } = new ObservableCollection<InvasionViewModel>();
        public ObservableCollection<VoidTradeViewModel> VoidTrades { get; } = new ObservableCollection<VoidTradeViewModel>();
        public ObservableCollection<VoidItemViewModel> VoidTradeItems { get; } = new ObservableCollection<VoidItemViewModel>();
        public ObservableCollection<DailyDealViewModel> DailyDeals { get; } = new ObservableCollection<DailyDealViewModel>();
        public ObservableCollection<BuildViewModel> Builds { get; } = new ObservableCollection<BuildViewModel>();

        public EarthTimeViewModel EarthTime { get; } = new EarthTimeViewModel();
        public EarthTimeViewModel CetusTime { get; } = new EarthTimeViewModel();
        public EarthTimeViewModel EidolonTime { get; } = new EarthTimeViewModel();

        public void AddNews(PostViewModel post) => News.Add(post);
        public PostViewModel TryGetNewsByDescription(string description) => News.FirstOrDefault(a => a.Description == description);
        public void RemoveNewsByTitle(string description)
        {
            PostViewModel post = TryGetNewsByDescription(description);
            if (post != null)
                News.Remove(post);
        }


        public void AddAlert(AlertViewModel alert) => Alerts.Add(alert);
        public AlertViewModel TryGetAlertById(Id id) => Alerts.FirstOrDefault(a => a.Id == id);
        public void RemoveAlertById(Id id)
        {
            AlertViewModel alert = TryGetAlertById(id);
            if (alert != null)
                Alerts.Remove(alert);
        }

        public void AddInvasion(InvasionViewModel invasion) => Invasions.Add(invasion);
        public InvasionViewModel TryGetInvasionById(Id id) => Invasions.FirstOrDefault(i => i.Id == id);
        public void RemoveInvasionById(Id id)
        {
            InvasionViewModel invasion = TryGetInvasionById(id);
            if (invasion != null)
                Invasions.Remove(invasion);
        }

        public void AddVoidTrade(VoidTradeViewModel trader) => VoidTrades.Add(trader);
        public VoidTradeViewModel TryGetVoidTradeById(Id id) => VoidTrades.FirstOrDefault(i => i.Id == id);
        public void RemoveVoidTradeById(Id id)
        {
            VoidTradeViewModel trader = TryGetVoidTradeById(id);
            if (trader != null)
                VoidTrades.Remove(trader);
        }

        public void AddVoidTradeItem(VoidItemViewModel manifest) => VoidTradeItems.Add(manifest);
        public VoidItemViewModel TryGetVoidTradeItemByName(string name) => VoidTradeItems.FirstOrDefault(i => i.ItemType == name);
        public void RemoveVoidTradeItemByName(string name)
        {
            VoidItemViewModel item = TryGetVoidTradeItemByName(name);
            if (item != null)
                VoidTradeItems.Remove(item);
        }

        public void RemoveAllVoidTraderItems()
        {
            for (var i = VoidTradeItems.Count-1; i >= 0; i--)
            {
                var item = VoidTradeItems[i];
                VoidTradeItems.Remove(item);
            }
        }

        public void AddDailyDeal(DailyDealViewModel deal) => DailyDeals.Add(deal);
        public DailyDealViewModel TryGetDailyDealByName(string name) => DailyDeals.FirstOrDefault(i => i.StoreItemOriginal == name);
        public void RemoveDailyDealByName(string name)
        {
            DailyDealViewModel deal = TryGetDailyDealByName(name);
            if (deal != null)
                DailyDeals.Remove(deal);
        }

        public void AddBuild(BuildViewModel build) => Builds.Add(build);
        public BuildViewModel TryGetBuildById(int id) => Builds.FirstOrDefault(i => i.Id == id);
        public void RemoveBuildById(int id)
        {
            BuildViewModel build = TryGetBuildById(id);
            if (build != null)
                Builds.Remove(build);
        }

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