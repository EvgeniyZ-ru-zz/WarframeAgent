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

        public ObservableCollection<AlertViewModel> Alerts => GameView.Alerts;
        public ObservableCollection<Invasion> Invasions => GameView.Invasions;
        public News NewsData { get; }

        public HomeViewModel(GameViewModel gameView, News newsData)
        {
            GameView = gameView;
            NewsData = newsData;
        }
    }
}
