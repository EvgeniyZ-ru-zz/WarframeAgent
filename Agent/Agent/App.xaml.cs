using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Agent.View;
using Core;
using Core.Events;

using Agent.ViewModel;
using NLog;

namespace Agent
{
    public partial class App : Application
    {
        MainViewModel mainVM;
        View.SplashScreen splashScreen;
        View.MainWindow mainWindow;

        /// <summary>The event mutex name.</summary>
        private const string UniqueEventName = "7330f03f-38d8-40bc-b123-fba47f61a7e5";

        /// <summary>The unique mutex name.</summary>
        private const string UniqueMutexName = "7330f03f-38d8-40bc-b123-fba47f61a7e4";

        /// <summary>The event wait handle.</summary>
        private EventWaitHandle eventWaitHandle;

        /// <summary>The mutex.</summary>
        private Mutex mutex;

        public App()
        {
            CheckWindow();
        }

        private void CheckWindow()
        {


            this.mutex = new Mutex(true, UniqueMutexName, out bool isOwned);
            this.eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            // So, C# would not give a warning that this variable is not used.
            GC.KeepAlive(this.mutex);

            if (isOwned)
            {
                // Spawn a thread which will be waiting for our event
                var thread = new Thread(
                    () =>
                    {
                        while (this.eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() => ((MainWindow)Current.MainWindow).BringToForeground()));
                        }
                    })
                {

                    // It is important mark it as background otherwise it will prevent app from exiting.
                    IsBackground = true
                };
                thread.Start();
                return;
            }

            // Notify other instance so it could bring itself to foreground.
            this.eventWaitHandle.Set();

            // Terminate this instance.
            this.Shutdown();
        }

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
            DispatcherUnhandledException += AppDispatcherUnhandledException;

            Tools.Logging.Send(LogLevel.Trace, $"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Tools.Logging.Send(LogLevel.Trace, $"OS: {Environment.OSVersion}");

            Settings.Load(); //Подгружаем настройки
            Settings.Program.Data.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Settings.Program.Save();
            mainVM = new MainViewModel();

            if (!Settings.Program.Configure.UseGpu || ForceSoftwareRendering)
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            var splashVM = new SplashViewModel(mainVM);
            splashVM.Exited += OnSplashExited;
            splashScreen = new View.SplashScreen() { DataContext = splashVM };
            splashScreen.Show();
            splashVM.Run();
        }

        #region Exception

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Tools.Logging.Send(LogLevel.Fatal, "An application error occurred.", e.Exception);
#if DEBUG // In debug mode do not custom-handle the exception, let Visual Studio handle it
            e.Handled = false;
#else
                ShowUnhandledException(e); 
            #endif
        }

        private void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var errorMessage =
                $"An application error occurred.\nPlease check whether your data is correct and repeat the action. If this error occurs again there seems to be a more serious malfunction in the application, and you better close it.\n\nError: {e.Exception.Message + (e.Exception.InnerException != null ? "\n" + e.Exception.InnerException.Message : null)}";

            MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
        }

        #endregion

        void OnSplashExited(object sender, SplashExitedEventArgs e)
        {
            if (e.AllowApplicationRun)
            {
                BackgroundEvent.Start();

                // need to open main window before closing splash, otherwise application
                // will lose its focus
                mainVM.IsConnectionLost = !e.HasConnection;
                mainWindow = new View.MainWindow() { DataContext = mainVM };
                mainWindow.Closed += (o, args) => CleanupExit();
                mainWindow.Show();
                mainVM.Run();
            }

            splashScreen.Close();
            splashScreen = null;

            if (!e.AllowApplicationRun)
                CleanupExit();
        }

        async void CleanupExit()
        {
            await mainVM.StopAsync();
            Shutdown(1);
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