using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

using Core;
using Core.Events;

using Agent.ViewModel;

namespace Agent
{
    /// <summary>
    ///     Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainViewModel mainVM;
        View.SplashScreen splashScreen;
        View.MainWindow mainWindow;

        public bool ForceSoftwareRendering
        {
            get
            {
                int renderingTier = (RenderCapability.Tier >> 16);
                return renderingTier == 0;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings.Load(); //Подгружаем настройки
            mainVM = new MainViewModel();

            if (Settings.Program.Core.UseGpu)
            {
                if (ForceSoftwareRendering)
                    RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
            else
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }

            var splashVM = new SplashViewModel(mainVM);
            splashVM.Exited += OnSplashExited;
            splashScreen = new View.SplashScreen() { DataContext = splashVM };
            splashScreen.Show();
            splashVM.Run();
        }

        void OnSplashExited(object sender, SplashExitedEventArgs e)
        {
            if (e.AllowApplicationRun)
            {
                BackgroundEvent.Start();

                // need to open main window before closing splash, otherwise application
                // will exit (due to ShutdownMode="OnLastWindowClose")
                mainVM.IsConnectionLost = !e.HasConnection;
                mainWindow = new View.MainWindow() { DataContext = mainVM };
                mainWindow.Show();
                mainVM.Run();
            }

            splashScreen.Close();
            splashScreen = null;
        }

        #region ResizeWindows

        private bool _resizeInProcess;

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                _resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                _resizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (_resizeInProcess)
            {
                var senderRect = sender as Rectangle;
                var mainWindow = senderRect.Tag as Window;
                if (senderRect != null)
                {
                    var width = e.GetPosition(mainWindow).X;
                    var height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        width += 5;
                        if (width > 0)
                            mainWindow.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        width -= 5;
                        mainWindow.Left += width;
                        width = mainWindow.Width - width;
                        if (width > 0)
                            mainWindow.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        height += 5;
                        if (height > 0)
                            mainWindow.Height = height;
                    }
                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        height -= 5;
                        mainWindow.Top += height;
                        height = mainWindow.Height - height;
                        if (height > 0)
                            mainWindow.Height = height;
                    }
                }
            }
        }

        #endregion
    }
}