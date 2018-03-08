using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Repair
{
    public partial class MainWindow : Window
    {
        public RepairProcess RepairProcess;

        public MainWindow()
        {
            InitializeComponent();
            RepairProcess = new RepairProcess();
            DataContext = RepairProcess;
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();

//#if DEBUG
//            args = new[] {"", "/md5"};
//#endif

            if (args.Length > 1)
            {
                await RepairProcess.Start(args[1]);
            }
            else
            {
                await RepairProcess.Start();
                Application.Current.Shutdown();
            }
        }
    }
}
