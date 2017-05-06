using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Core;
using System.Threading.Tasks;
using static Agent.Events.GlobalEvents;

namespace Agent
{
    /// <summary>
    ///     Логика взаимодействия для SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        //DispatcherTimer dT = new DispatcherTimer();
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_OnLoaded(object sender, RoutedEventArgs e)
        {
            Connection.Start();
            GameDataUpdate.Start();
            GameDataUpdate.Updated += GameDataUpdate_Updated;
            Connection.Disconnected += Connection_Disconnected;
        }

        private void Connection_Disconnected()
        {
            MessageBox.Show("Невозможно получить данные.");
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Close();
            });
        }

        private void GameDataUpdate_Updated()
        {
            GameDataUpdate.Updated -= GameDataUpdate_Updated;
            Connection.Disconnected -= Connection_Disconnected;
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                var main = new MainWindow();
                main.Show();
                Close();
            });
        }
    }
}
