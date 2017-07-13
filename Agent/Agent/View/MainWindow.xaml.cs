using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;
using Filters = Core.ViewModel.Filters;

namespace Agent.View
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        public static Game GameData = new Game();
        public static News NewsData = new News();
        public static GameViewModel GameView = new GameViewModel();
        public static GlobalEvents.GameDataEvent GameDataEvent = new GlobalEvents.GameDataEvent();
        public static NotificationModel NotificationWatcherwatcher = new NotificationModel();

        public MainWindow(Visibility visibility)
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NewsData.Load($"{Settings.Program.Directories.Temp}/NewsData.json");
            InitializeComponent();
            InitializeAnimation();

            ConnLostImg.Visibility = visibility;
        }

        public MainWindow()
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NewsData.Load($"{Settings.Program.Directories.Temp}/NewsData.json");
            InitializeComponent();
            InitializeAnimation();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            ThemeChange(Settings.Program.Theme);
            BgImg.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/Resources/Images/Background/{Settings.Program.BackgroundId}.jpg"));

            MainFrame.Navigate(new Uri("View/HomePage.xaml", UriKind.Relative));

            GameDataEvent.Connected += GameDataEvent_Connected;
            GameDataEvent.Disconnected += GameDataEvent_Disconnected;
            GameDataEvent.Updated += GameDataEvent_Updated;
            BackgroundEvent.Changed += BackgroundEvent_Changed;
            NotificationWatcherwatcher.AlertNotificationArrived += NotificationWatcherwatcherOnAlertNotificationArrived;
            NotificationWatcherwatcher.AlertNotificationDeparted +=
            NotificationWatcherwatcherOnAlertNotificationDeparted;
            NotificationWatcherwatcher.Start(GameData);
        }


        private void NotificationWatcherwatcherOnAlertNotificationDeparted(object sender,
            RemovedAlertNotificationEventArgs e)
        {
            Application.Current.Dispatcher?.InvokeAsync(() =>
            {
                GameView.Alerts.Remove(e.Notification);
            });

            Debug.WriteLine($"Удаляю тревогу {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");
        }

        private void NotificationWatcherwatcherOnAlertNotificationArrived(object sender,
            NewAlertNotificationEventArgs e)
        {
            var ntfVm = new NotificationViewModel(e.Notification);
            Debug.WriteLine($"Новая тревога {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");
            Debug.WriteLine($"Переводим значение {ntfVm.Text}!", $"[{DateTime.Now}]");

            #region Переводим предмет

            string revardValue = null;

            if (e.Notification.MissionInfo.MissionReward.CountedItems != null)
            {
                var item = e.Notification.MissionInfo.MissionReward.CountedItems[0];
                var itemCount = item.ItemCount >= 2 ? $"[{item.ItemCount}]" : string.Empty;
                revardValue = $"{item.ItemType.GetFilter(Filters.FilterType.Item)} {itemCount}";
            }
            else if (e.Notification.MissionInfo.MissionReward.Items != null)
            {
                revardValue = e.Notification.MissionInfo.MissionReward.Items[0].GetFilter(Filters.FilterType.Item);
            }

            e.Notification.MissionInfo.Reward = revardValue;

            #endregion

            e.Notification.MissionInfo.Faction = e.Notification.MissionInfo.Faction.GetFilter(Filters.FilterType.Fraction);
            e.Notification.MissionInfo.Planet = e.Notification.MissionInfo.Location.GetFilter(Filters.FilterType.Planet).ToUpper().Split('|');
            e.Notification.MissionInfo.MissionType = e.Notification.MissionInfo.MissionType.GetFilter(Filters.FilterType.Mission);


            Application.Current.Dispatcher?.InvokeAsync(() =>
            {
                if (GameView.Alerts == null) GameView.Alerts = new ObservableCollection<Alert>();
                GameView.Alerts.Add(e.Notification);
            });
        }

        private void BackgroundEvent_Changed()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart) delegate
                {
                    BgImg.Source = new BitmapImage(new Uri(
                        $"pack://application:,,,/Resources/Images/Background/{Settings.Program.BackgroundId}.jpg"));
                });
        }

        private void GameDataEvent_Updated()
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NotificationWatcherwatcher.Start(GameData);
            Debug.WriteLine(GameData.Data.Alerts.Count, $"Alerts [{DateTime.Now}]");
        }

        #region Взаимодействие с окном

        //Перетаскиваем окно мышью.
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #endregion


        private void StyleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is Button srcButton)) return;
            var name = srcButton.Name;
            if (name == "ChangeBg")
            {
                BackgroundEvent.Restart();
                return;
            }
            if (name == "ThemeBtn")
            {
                var res = MessageBox.Show(
                    "При смене темы могут возникнуть \"артефакты\".\nРекомендую перезапустить приложение.",
                    "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (Settings.Program.Theme == Themes.Dark)
                {
                    if (res == MessageBoxResult.OK) ThemeChange(Themes.Light);
                }
                else
                {
                    if (res == MessageBoxResult.OK) ThemeChange(Themes.Dark);
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.Source is RadioButton srcButton)) return;
            var name = srcButton.Name;
            MainFrame.Navigate(
                new Uri("View/" + name.Substring(0, name.Length - 3) + "Page.xaml", UriKind.Relative));
        }

        #region События

        private void GameDataEvent_Disconnected()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate { ConnLostImg.Visibility = Visibility.Visible; });
        }

        private void GameDataEvent_Connected()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate { ConnLostImg.Visibility = Visibility.Collapsed; });
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine($"w.{e.NewSize.Width} h.{e.NewSize.Height}");
            ResetPopUp();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            ResetPopUp();
        }

        private void ResetPopUp()
        {
            var offset = MyPopup.HorizontalOffset;
            MyPopup.HorizontalOffset = offset + 1;
            MyPopup.HorizontalOffset = offset;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Settings.Program.Save();
        }

        /// <summary>
        ///     Изменение темы приложения
        /// </summary>
        /// <param name="theme">Темная или светлая</param>
        private void ThemeChange(Themes theme)
        {
            var uri = new Uri($"Styles/Theme/{theme}.xaml", UriKind.Relative);
            var resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            Settings.Program.Theme = theme;
            Settings.Program.Save();
        }

        #endregion

        #region Закрытие и сворачивание

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Program.Save();
            Close();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Анимация

        private void InitializeAnimation()
        {
            LeftPanelAnimation(); //Анимация боковой панели
        }

        #region Боковая панель

        private void LeftPanelAnimation()
        {
            LeftPanelTop.Opacity = 0;
            LeftPanelTheme.Opacity = 0;
            LeftPanelBottom.Opacity = 0;
            var animation = new Storyboard();
            var a = new DoubleAnimation
            {
                From = 0,
                To = 40,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            a.Completed += LeftPanelAnimation_Completed;
            Storyboard.SetTarget(a, LeftPanelGrid);
            Storyboard.SetTargetProperty(a, new PropertyPath(WidthProperty));
            animation.Children.Add(a);
            animation.Begin();
        }

        private void LeftPanelAnimation_Completed(object sender, EventArgs e)
        {
            LeftPanelTheme.Opacity = 0;
            LeftPanelBottom.Opacity = 0;
            var animation = new Storyboard();
            var top = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            var theme = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            var bottom = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            Storyboard.SetTarget(top, LeftPanelTop);
            Storyboard.SetTarget(theme, LeftPanelTheme);
            Storyboard.SetTarget(bottom, LeftPanelBottom);

            Storyboard.SetTargetProperty(top, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(theme, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(bottom, new PropertyPath(OpacityProperty));

            animation.Children.Add(top);
            animation.Children.Add(theme);
            animation.Children.Add(bottom);

            animation.Begin();
        }

        #endregion

        #endregion
    }
}