using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
            Animation animation = new Animation(this);
            InitializeComponent();
            animation.InitializeAnimation();

            ConnLostImg.Visibility = visibility;
        }

        public MainWindow()
        {
            GameData.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            NewsData.Load($"{Settings.Program.Directories.Temp}/NewsData.json");
            Animation animation = new Animation(this);
            InitializeComponent();
            animation.InitializeAnimation();
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
            #region Переводим предмет

            string rewardValue = null;
            string rewardType = null;

            if (e.Notification.MissionInfo.MissionReward.CountedItems != null)
            {
                var item = e.Notification.MissionInfo.MissionReward.CountedItems[0];
                var itemCount = item.ItemCount >= 2 ? $"[{item.ItemCount}]" : string.Empty;
                var reward = item.ItemType.GetFilter(Filters.FilterType.Item).FirstOrDefault();

                rewardType = reward.Value;
                rewardValue = $"{reward.Key} {itemCount}";
            }
            else if (e.Notification.MissionInfo.MissionReward.Items != null)
            {
                var reward = e.Notification.MissionInfo.MissionReward.Items[0].GetFilter(Filters.FilterType.Item)
                    .FirstOrDefault();

                rewardType = reward.Value;
                rewardValue = reward.Key;
            }

            e.Notification.MissionInfo.Reward = rewardValue;

            switch (rewardType)
            {
                case "Шлема":
                    e.Notification.MissionInfo.RewardColor = Brushes.BlueViolet;
                    break;
                case "Чертежи":
                    e.Notification.MissionInfo.RewardColor = Brushes.BlueViolet;
                    break;
                case "Ауры":
                    e.Notification.MissionInfo.RewardColor = Brushes.OrangeRed;
                    break;
                case "Модификаторы":
                    e.Notification.MissionInfo.RewardColor = Brushes.DarkCyan;
                    break;
                default:
                    e.Notification.MissionInfo.RewardColor = (Brush) Application.Current.Resources["TextColor"]; 
                    break;
            }

            #endregion
            
            e.Notification.MissionInfo.Faction = e.Notification.MissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.MissionInfo.Planet = e.Notification.MissionInfo.Location.GetFilter(Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            e.Notification.MissionInfo.MissionType = e.Notification.MissionInfo.MissionType.GetFilter(Filters.FilterType.Mission).FirstOrDefault().Key;


            Application.Current.Dispatcher?.InvokeAsync(() =>
            {
                if (GameView.Alerts == null) GameView.Alerts = new ObservableCollection<Alert>();
                GameView.Alerts.Add(e.Notification);
            });
        }

        private void BackgroundEvent_Changed()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate
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

 
    }
}