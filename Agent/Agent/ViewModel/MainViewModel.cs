using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class MainViewModel : VM
    {
        private Game GameData = new Game();
        private GameViewModel GameView = new GameViewModel();
        private AlertsEngine AlertsEngine;
        private InvasionsEngine InvasionsEngine;

        public GlobalEvents.GameDataEvent GameDataEvent = new GlobalEvents.GameDataEvent();
        public NotificationModel NotificationWatcherwatcher = new NotificationModel();

        public HomeViewModel HomeViewModel { get; }
        public News NewsData { get; }

        public MainViewModel()
        {
            GameView = new GameViewModel();
            AlertsEngine = new AlertsEngine(GameView);
            InvasionsEngine = new InvasionsEngine(GameView);
            GameDataEvent.Start();

            NewsData = new News();
            HomeViewModel = new HomeViewModel(GameView, NewsData);

            ActivateHomeCommand = new RelayCommand(() => CurrentContent = HomeViewModel);
            ActivateNewsCommand = new RelayCommand(() => CurrentContent = NewsData);
            CurrentContent = HomeViewModel;
        }

        public void Run()
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NewsData.Load($"{Settings.Program.Directories.Temp}/NewsData.json");

            GameDataEvent.Connected += GameDataEvent_Connected;
            GameDataEvent.Disconnected += GameDataEvent_Disconnected;
            GameDataEvent.Updated += GameDataEvent_Updated;
            AlertsEngine.Run(NotificationWatcherwatcher);
            InvasionsEngine.Run(NotificationWatcherwatcher);
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

        VM currentContent;
        public VM CurrentContent
        {
            get => currentContent;
            set => Set(ref currentContent, value);
        }

        public ICommand ActivateHomeCommand { get; }
        public ICommand ActivateNewsCommand { get; }
    }
}
