using System;
using System.IO;
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

#if DEBUG
            args = new[] { "", "Md51", "Release"};
#endif

            RepairProcess.Mod selectMod = RepairProcess.Mod.Default;
            RepairProcess.Revision revision = RepairProcess.Revision.Release;

            if (args.Length > 1)
            {
                if (!Enum.TryParse(args[1], out selectMod))
                {
                    selectMod = RepairProcess.Mod.Default;
                }

                if (selectMod == RepairProcess.Mod.Default)
                {
                    Enum.TryParse(args[2], out revision);
                }
            }

            await RepairProcess.Start(selectMod, revision);
        }
    }
}
