using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Core;
using Core.Events;

namespace Agent.View
{
    public partial class SplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_OnLoaded(object sender, RoutedEventArgs e)
        {
            BackgroundEvent.Start();
            MainWindow.GameDataEvent.Start();
            MainWindow.GameDataEvent.Updated += GameDataEvent_Updated;
            MainWindow.GameDataEvent.Disconnected += GameDataEvent_Disconnected;
        }

        #region События

        private void GameDataEvent_Disconnected()
        {
            if (File.Exists($"{Settings.Program.Directories.Temp}/GameData.json") &&
                File.Exists($"{Settings.Program.Directories.Temp}/NewsData.json"))
            {
                MainWindow.GameDataEvent.Updated -= GameDataEvent_Updated;
                MainWindow.GameDataEvent.Disconnected -= GameDataEvent_Disconnected;
                var message = MessageBox.Show(
                    "Невозможно получить данные с сервера.\nНо нам удалось найти старые данные.\nПоказать?",
                    "Внимание!",
                    MessageBoxButton.OKCancel);

                if (message == MessageBoxResult.OK)
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate
                    {
                        var main = new MainWindow(Visibility.Visible);
                        main.Show();
                        Close();
                    });
                else
                    Environment.Exit(0);
            }
            else
            {
                MessageBox.Show("Невозможно получить данные с сервера.");
                Environment.Exit(0);
            }
        }

        private void GameDataEvent_Updated()
        {
            MainWindow.GameDataEvent.Updated -= GameDataEvent_Updated;
            MainWindow.GameDataEvent.Disconnected -= GameDataEvent_Disconnected;
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate
            {
                var main = new MainWindow();
                main.Show();
                Close();
            });
        }

        #endregion
    }
}