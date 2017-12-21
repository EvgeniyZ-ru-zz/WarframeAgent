using System.Collections.ObjectModel;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class HomeViewModel : VM
    {
        private GameViewModel GameView;

        public ObservableCollection<PostViewModel> News => GameView.News;
        public ObservableCollection<AlertViewModel> Alerts => GameView.Alerts;
        public ObservableCollection<InvasionViewModel> Invasions => GameView.Invasions;
        public ObservableCollection<VoidTradeViewModel> VoidTrades => GameView.VoidTrades;
        public ObservableCollection<VoidItemViewModel> VoidTradeItems => GameView.VoidTradeItems;
        public ObservableCollection<DailyDealViewModel> DailyDeals => GameView.DailyDeals;
        public ObservableCollection<BuildViewModel> Builds => GameView.Builds;
        public LocationTimeViewModel EarthTime => GameView.EarthTime;
        public LocationTimeViewModel CetusTime => GameView.CetusTime;
        public LocationTimeViewModel EidolonTime => GameView.EidolonTime;

        public HomeViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }
    }
}
