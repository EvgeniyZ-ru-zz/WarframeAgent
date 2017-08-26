using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Agent.ViewModel;
using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;
using FontAwesome.WPF;

namespace Agent.View
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        private static Animation animation;
        public MainWindow()
        {
            animation = new Animation(this);
            InitializeComponent();
            animation.InitializeAnimation();
            //ShowPopUp("Привет мир!", FontAwesomeIcon.Amazon, "#FF3782CD");
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            ThemeChange(Settings.Program.Configure.Theme);
            ReloadBackground(Settings.Program.Data.BackgroundId);
            BackgroundEvent.Changed += BackgroundEvent_Changed;
        }


        public void BringToForeground()
        {
            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }

        public void ShowPopUp(string text, FontAwesomeIcon icon, string color)
        {
            var convertFromString = ColorConverter.ConvertFromString(color);
            if (convertFromString != null) PopUpPanel.BorderBrush = new SolidColorBrush((Color)convertFromString);
            PopUpIcon.Icon = icon;
            PopUpText.Text = text;
            animation.PopUpAnimation();
        }

        private async void BackgroundEvent_Changed(int newBackgroundId)
        {
            await AsyncHelpers.RedirectToMainThread();
            ReloadBackground(newBackgroundId);
        }

        // TODO: move this into a separate backgournd control
        void ReloadBackground(int newBackgroundId)
        {
            BgImg.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/Resources/Images/Background/{newBackgroundId}.jpg"));
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
                if (Settings.Program.Configure.Theme == Themes.Dark)
                {
                    if (res == MessageBoxResult.OK) ThemeChange(Themes.Light);
                }
                else
                {
                    if (res == MessageBoxResult.OK) ThemeChange(Themes.Dark);
                }
            }
        }

        #region События
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine($"w.{e.NewSize.Width} h.{e.NewSize.Height}");
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
            Settings.Program.Configure.Theme = theme;
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