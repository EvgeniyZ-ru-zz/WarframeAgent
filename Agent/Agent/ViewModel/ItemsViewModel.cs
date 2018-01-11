using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.ViewModel;

namespace Agent.ViewModel
{
    class ItemsViewModel : VM
    {
        private GameViewModel GameView;

        public ItemsViewModel(GameViewModel gameView)
        {
            GameView = gameView;
        }
    }
}
