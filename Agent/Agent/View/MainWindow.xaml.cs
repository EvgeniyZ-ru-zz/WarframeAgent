using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Agent.ViewModel;
using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;

namespace Agent.View
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var animation = new Animation(this);
            InitializeComponent();
            animation.InitializeAnimation();
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            ThemeChange(Settings.Program.Theme);
            ReloadBackground();
            BackgroundEvent.Changed += BackgroundEvent_Changed;
        }

        private async void BackgroundEvent_Changed()
        {
            await AsyncHelpers.RedirectToMainThread();
            ReloadBackground();
        }

        // TODO: move this into a separate backgournd control
        void ReloadBackground()
        {
            BgImg.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/Resources/Images/Background/{Settings.Program.BackgroundId}.jpg"));
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

        #region События
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