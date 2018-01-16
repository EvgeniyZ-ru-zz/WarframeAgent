using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agent.ViewModel.Util;
using Core.ViewModel;

namespace Agent.ViewModel
{
    public class ItemsViewModel : VM
    {
        private GameViewModel GameView;

        public ObservableCollection<ItemGroupViewModel> Groups => GameView.ItemGroups;

        public ItemsViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }
    }
}
