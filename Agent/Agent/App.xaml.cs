using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Agent.View;
using Core;
using Agent.ViewModel;
using Microsoft.HockeyApp;
using NLog;

namespace Agent
{
    public partial class App : Application
    {
        MainViewModel mainVM;
        View.SplashScreen splashScreen;
        View.MainWindow mainWindow;

        /// <summary>The event mutex name.</summary>
        private const string UniqueEventName = "7330f03f-38d8-40bc-b123-fba47f61a7e1";

        /// <summary>The unique mutex name.</summary>
        private const string UniqueMutexName = "7330f03f-38d8-40bc-b123-fba47f61a7e2";

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
            mutex = new Mutex(true, UniqueMutexName, out bool isOwned);
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            // So, C# would not give a warning that this variable is not used.
            GC.KeepAlive(mutex);

            if (isOwned)
            {
                // Spawn a thread which will be waiting for our event
                var thread = new Thread(
                    () =>
                    {
                        while (eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() => mainWindow.BringToForeground()));
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
            eventWaitHandle.Set();

            // Terminate this instance.
            Shutdown();
        }

        public bool ForceSoftwareRendering
        {
            get
            {
                int renderingTier = RenderCapability.Tier >> 16;
                return renderingTier == 0;
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherUnhandledException += AppDispatcherUnhandledException;

            Tools.Logging.Send(LogLevel.Trace, $"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Tools.Logging.Send(LogLevel.Trace, $"OS: {Environment.OSVersion}");

            HockeyClient.Current.Configure("847a769d61234e969e7d4b321877e67c").SetExceptionDescriptionLoader(ex => "Exception HResult: " + ex.HResult);
            await HockeyClient.Current.SendCrashesAsync();

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
                // need to open main window before closing splash, otherwise application
                // will lose its focus
                mainVM.IsConnectionLost = !e.HasConnection;
                mainWindow = new MainWindow { DataContext = mainVM };
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
            if (sender is Rectangle senderRect)
            {
                _resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle senderRect)
            {
                _resizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (_resizeInProcess)
            {
                if (sender is Rectangle senderRect && senderRect.Tag is Window mWind)
                {
                    var width = e.GetPosition(mWind).X;
                    var height = e.GetPosition(mWind).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        width += 5;
                        if (width > 0)
                            mWind.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        width -= 5;
                        mWind.Left += width;
                        width = mWind.Width - width;
                        if (width > 0)
                            mWind.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        height += 5;
                        if (height > 0)
                            mWind.Height = height;
                    }
                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        height -= 5;
                        mWind.Top += height;
                        height = mWind.Height - height;
                        if (height > 0)
                            mWind.Height = height;
                    }
                }
            }
        }

#endregion
    }
}