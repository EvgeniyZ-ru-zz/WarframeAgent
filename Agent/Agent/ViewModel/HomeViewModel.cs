using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core;
using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class HomeViewModel : VM
    {
        private GameViewModel GameView;

        public ObservableCollection<PostViewModel> News => GameView.News;
        public ObservableCollection<AlertViewModel> Alerts => GameView.Alerts;
        public ObservableCollection<InvasionViewModel> Invasions => GameView.Invasions;
        public ObservableCollection<BuildViewModel> Builds => GameView.Builds;

        public HomeViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }
    }
}
