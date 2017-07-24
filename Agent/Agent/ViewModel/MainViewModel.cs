using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class MainViewModel : VM
    {
        private Game GameData = new Game();
        public News NewsData { get; }
        public GameViewModel GameView { get; }
        public AlertsViewModel AlertsViewModel { get; }
        public InvasionsViewModel InvasionsViewModel { get; }
        public GlobalEvents.GameDataEvent GameDataEvent = new GlobalEvents.GameDataEvent();
        public NotificationModel NotificationWatcherwatcher = new NotificationModel();

        public MainViewModel()
        {
            NewsData = new News();
            GameView = new GameViewModel();
            AlertsViewModel = new AlertsViewModel(GameView);
            InvasionsViewModel = new InvasionsViewModel(GameView);
        }

        public void Run()
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NewsData.Load($"{Settings.Program.Directories.Temp}/NewsData.json");

            GameDataEvent.Connected += GameDataEvent_Connected;
            GameDataEvent.Disconnected += GameDataEvent_Disconnected;
            GameDataEvent.Updated += GameDataEvent_Updated;
            AlertsViewModel.Run(NotificationWatcherwatcher);
            InvasionsViewModel.Run(NotificationWatcherwatcher);
            NotificationWatcherwatcher.Start(GameData);
        }

        private void GameDataEvent_Updated()
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NotificationWatcherwatcher.Start(GameData);
            Debug.WriteLine(GameData.Data.Alerts.Count, $"Alerts [{DateTime.Now}]");
        }

        private async void GameDataEvent_Disconnected()
        {
            await AsyncHelpers.RedirectToMainThread();
            IsConnectionLost = true;
        }

        private async void GameDataEvent_Connected()
        {
            await AsyncHelpers.RedirectToMainThread();
            IsConnectionLost = false;
        }

        bool isConnectionLost = false;
        public bool IsConnectionLost
        {
            get => isConnectionLost;
            set => Set(ref isConnectionLost, value);
        }
    }
}
