using System.Collections.ObjectModel;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class AlertsViewModel : VM
    {
        private GameViewModel GameView;

        public ObservableCollection<AlertViewModel> Alerts => GameView.Alerts;

        public AlertsViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }
    }
}
