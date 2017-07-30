using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Core.Model;

namespace Agent.ViewModel
{
    public class GameViewModel
    {
        public GameViewModel(GameModel model)
        {
            var reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            reloadTimer.Tick += reloadTimer_Elapsed;
            reloadTimer.Start();

            Model = model;
            AlertsEngine = new AlertsEngine(this);
            InvasionsEngine = new InvasionsEngine(this);
        }

        public void Run()
        {
            AlertsEngine.Run(Model);
            InvasionsEngine.Run(Model);
        }

        private AlertsEngine AlertsEngine;
        private InvasionsEngine InvasionsEngine;
        private GameModel Model;

        public ObservableCollection<Alert> Alerts { get; } = new ObservableCollection<Alert>();
        public ObservableCollection<Invasion> Invasions { get; } = new ObservableCollection<Invasion>();

        public void AddAlert(Alert alert) => Alerts.Add(alert);
        public void RemoveAlert(Alert alert) => Alerts.Remove(alert);

        public void AddInvasion(Invasion invasion) => Invasions.Add(invasion);
        public void RemoveAlert(Invasion invasion) => Invasions.Remove(invasion);

        private void reloadTimer_Elapsed(object sender, EventArgs e)
        {
            for (var index = 0; index < (Alerts).Count; index++)
            {
                var item = (Alerts)[index];
                item.Status = null; //?
            }
        }
    }
}