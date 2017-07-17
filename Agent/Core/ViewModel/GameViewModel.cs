using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Core.Model;

namespace Core.ViewModel
{
    public class GameViewModel
    {
        public GameViewModel()
        {
            var reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            reloadTimer.Tick += reloadTimer_Elapsed;
            reloadTimer.Start();
        }

        public ObservableCollection<Alert> Alerts { get; set; }
        public ObservableCollection<Invasion> Invasions { get; set; }

        private void reloadTimer_Elapsed(object sender, EventArgs e)
        {
            if (Alerts == null) return;
            for (var index = 0; index < (Alerts).Count; index++)
            {
                var item = (Alerts)[index];
                item.Status = null;
            }
        }
    }
}