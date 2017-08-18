using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Agent.View;

namespace Agent
{
    class Animation
    {
        private MainWindow main;

        public Animation(MainWindow MainWindow)
        {
            main = MainWindow;
        }

        #region Анимация

        public void InitializeAnimation()
        {
            LeftPanelAnimation(); //Анимация боковой панели
        }

        #region Боковая панель

        private void LeftPanelAnimation()
        {
            main.LeftPanelTop.Opacity = 0;
            main.LeftPanelTheme.Opacity = 0;
            main.LeftPanelBottom.Opacity = 0;
            var animation = new Storyboard();
            var a = new DoubleAnimation
            {
                From = 0,
                To = 40,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            a.Completed += LeftPanelAnimation_Completed;
            Storyboard.SetTarget(a, main.LeftPanelGrid);
            Storyboard.SetTargetProperty(a, new PropertyPath(FrameworkElement.WidthProperty));
            animation.Children.Add(a);
            animation.Begin();
        }

        private void LeftPanelAnimation_Completed(object sender, EventArgs e)
        {
            main.LeftPanelTheme.Opacity = 0;
            main.LeftPanelBottom.Opacity = 0;
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

            Storyboard.SetTarget(top, main.LeftPanelTop);
            Storyboard.SetTarget(theme, main.LeftPanelTheme);
            Storyboard.SetTarget(bottom, main.LeftPanelBottom);

            Storyboard.SetTargetProperty(top, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTargetProperty(theme, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTargetProperty(bottom, new PropertyPath(UIElement.OpacityProperty));

            animation.Children.Add(top);
            animation.Children.Add(theme);
            animation.Children.Add(bottom);

            animation.Begin();
        }

        #endregion

        #region PopUp панель

        public void PopUpAnimation()
        {
            var animation = new ThicknessAnimation();
            animation.EasingFunction = new ExponentialEase();
            animation.To = new Thickness(5, 0, 5, 0);
            animation.Completed += PopUpCompleted;
            main.PopUpPanel.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private async void PopUpCompleted(object sender, EventArgs eventArgs)
        {
            await Task.Delay(TimeSpan.FromSeconds(3.5));
            var animation = new ThicknessAnimation();
            animation.EasingFunction = new ExponentialEase();
            animation.To = new Thickness(5, -40, 5, 0);
            animation.Duration = TimeSpan.FromSeconds(1);
            main.PopUpPanel.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        #endregion
        #endregion
    }
}
