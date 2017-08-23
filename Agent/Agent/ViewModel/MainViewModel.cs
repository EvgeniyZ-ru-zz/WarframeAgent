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
        private GameViewModel GameView;
        private ServerModel ServerModel;

        public GlobalEvents.ServerEvents ServerEvents = new GlobalEvents.ServerEvents();
        public GameModel GameModel = new GameModel();

        public HomeViewModel HomeViewModel { get; }
        public News NewsData { get; }

        public MainViewModel()
        {
            BadFilterReportModel.Start();
            ServerModel = new ServerModel(ServerEvents);
            ServerModel.Start();
            GameView = new GameViewModel(GameModel);

            NewsData = new News();
            HomeViewModel = new HomeViewModel(GameView, NewsData);

            ActivateHomeCommand = new RelayCommand(() => CurrentContent = HomeViewModel);
            ActivateNewsCommand = new RelayCommand(() => CurrentContent = NewsData);
            CurrentContent = HomeViewModel;
        }

        public void Run()
        {
            //GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NewsData.Load($"{Settings.Program.Directories.Temp}/NewsData.json");

            ServerEvents.Connected += GameDataEvent_Connected;
            ServerEvents.Disconnected += GameDataEvent_Disconnected;
            GameView.Run();
            GameModel.Start(ServerEvents, $"{Settings.Program.Directories.Temp}/GameData.json");
        }

        public async Task StopAsync()
        {
            await BadFilterReportModel.StopAsync();
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
