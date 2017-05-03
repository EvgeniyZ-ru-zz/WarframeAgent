using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Agent
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAnimation();

            //Data.Json.Load($"{Settings.Program.Directories.Temp}/GameData.json");
            //MessageBox.Show(Data.Json.Model.Alerts[0].Activation.date.NumberLong.ToString());

            var styles = new List<string> { "light", "dark" };
            styleBox.SelectionChanged += ThemeChange;
            styleBox.ItemsSource = styles;
            styleBox.SelectedItem = "light";
        }

        private void ThemeChange(object sender, SelectionChangedEventArgs e)
        {
            string style = styleBox.SelectedItem as string;
            // определяем путь к файлу ресурсов
            var uri = new Uri($"Styles/Theme/{style}.xaml", UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        #region Взаимодействие с окном

        //Перетаскиваем окно мышью.
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #endregion

        #region Закрытие и сворачивание

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //Settings.Program.Save();
            Close();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            //var alertPage = new AlertsPage();
            //alertPage._statsTimer.Stop(); //TODO: Включить, после создания. (обновление таймера в инфе таймера)
            //TaskbarIcon.Visibility = Visibility.Visible;
            //Hide();
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
            LeftPanelContent.Opacity = 0;
            var animation = new Storyboard();
            var a = new DoubleAnimation()
            {
                From = 0,
                To = 40,
                Duration = TimeSpan.FromSeconds(0.6),
            };
            a.Completed += LeftPanelAnimation_Completed;
            Storyboard.SetTarget(a, LeftPanelGrid);
            Storyboard.SetTargetProperty(a, new PropertyPath(WidthProperty));
            animation.Children.Add(a);
            animation.Begin();
        }

        private void LeftPanelAnimation_Completed(object sender, EventArgs e)
        {
            var animation = new Storyboard();
            var a = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTarget(a, LeftPanelContent);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            animation.Children.Add(a);
            animation.Begin();
        }
        #endregion

        #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine($"w.{e.NewSize.Width} h.{e.NewSize.Height}");
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Settings.Program.Save();
        }
    }
}
