using System.Collections.ObjectModel;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class NewsViewModel : VM
    {
        private GameViewModel GameView;

        public ObservableCollection<PostViewModel> News => GameView.News;

        public NewsViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }
    }
}
