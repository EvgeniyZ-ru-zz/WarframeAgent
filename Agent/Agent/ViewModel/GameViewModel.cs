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
            AlertsEngine = new AlertsEngine(this, filtersEvent);
            InvasionsEngine = new InvasionsEngine(this, filtersEvent);
        }

        public void Run()
        {
            AlertsEngine.Run(Model);
            InvasionsEngine.Run(Model);
        }

        private AlertsEngine AlertsEngine;
        private InvasionsEngine InvasionsEngine;
        private GameModel Model;

        public ObservableCollection<AlertViewModel> Alerts { get; } = new ObservableCollection<AlertViewModel>();
        public ObservableCollection<InvasionViewModel> Invasions { get; } = new ObservableCollection<InvasionViewModel>();

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

        private void reloadTimer_Elapsed(object sender, EventArgs e)
        {
            for (var index = 0; index < (Alerts).Count; index++)
            {
                var item = (Alerts)[index];
                item.UpdateStatus(); // TODO: make it inside the alert
            }
        }
    }
}