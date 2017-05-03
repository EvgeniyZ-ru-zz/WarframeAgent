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
            var task = new Task(() =>
            {
                if (Tools.Network.Ping(Settings.Program.Urls.Game))
                {
                    var dir = Settings.Program.Directories.Temp;
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    Tools.Network.DownloadFile(Settings.Program.Urls.Game, $"{Settings.Program.Directories.Temp}/GameData.json");
                    Thread.Sleep(500);            
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate
                    {
                        var main = new MainWindow();
                        main.Show();
                        Close();
                    });
                }
                else
                {
                    MessageBox.Show("Не удается получить доступ к ресурсу...");
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) Close);
                }
            });
            task.Start();
        }
    }
}
