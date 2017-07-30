using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Agent.View;
using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    class AlertsEngine
    {
        private GameViewModel GameView;

        public AlertsEngine(GameViewModel gameView)
        {
            GameView = gameView;
        }

        public void Run(GameModel model)
        {
            model.AlertNotificationArrived += AddEvent;
            model.AlertNotificationDeparted += RemoveEvent;
            // TODO: race condition with arriving events; check if event is already there
            foreach (var alert in model.GetCurrentAlerts())
            {
                PrepareAlert(alert);
                GameView.AddAlert(alert);
            }
        }

        private async void AddEvent(object sender, NewAlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

            var ntfVm = new NotificationViewModel(e.Notification); // TODO: replace alert with alertVM here!
            Debug.WriteLine($"Новая тревога {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            PrepareAlert(e.Notification);
            GameView.AddAlert(e.Notification);
        }

        void PrepareAlert(Alert alert)
        {
            #region Переводим предмет

            string rewardValue = null;
            string rewardType = null;

            if (alert.MissionInfo.MissionReward.CountedItems != null)
            {
                var item = alert.MissionInfo.MissionReward.CountedItems[0];
                var itemCount = item.ItemCount >= 2 ? $"[{item.ItemCount}]" : string.Empty;
                var reward = item.ItemType.GetFilter(Filters.FilterType.Item).FirstOrDefault();

                rewardType = reward.Value;
                rewardValue = $"{reward.Key} {itemCount}";
            }
            else if (alert.MissionInfo.MissionReward.Items != null)
            {
                var reward = alert.MissionInfo.MissionReward.Items[0].GetFilter(Filters.FilterType.Item)
                    .FirstOrDefault();

                rewardType = reward.Value;
                rewardValue = reward.Key;
            }

            alert.MissionInfo.Reward = rewardValue;

            switch (rewardType)
            {
            case "Шлема":
                alert.MissionInfo.RewardColor = Brushes.BlueViolet;
                break;
            case "Чертежи":
                alert.MissionInfo.RewardColor = Brushes.BlueViolet;
                break;
            case "Ауры":
                alert.MissionInfo.RewardColor = Brushes.OrangeRed;
                break;
            case "Модификаторы":
                alert.MissionInfo.RewardColor = Brushes.DarkCyan;
                break;
            default:
                alert.MissionInfo.RewardColor = (Brush)Application.Current.Resources["TextColor"];
                break;
            }

            #endregion

            alert.MissionInfo.Faction = alert.MissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            alert.MissionInfo.Planet = alert.MissionInfo.Location.GetFilter(Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            alert.MissionInfo.MissionType = alert.MissionInfo.MissionType.GetFilter(Filters.FilterType.Mission).FirstOrDefault().Key;
        }

        private async void RemoveEvent(object sender, RemovedAlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Debug.WriteLine($"Удаляю тревогу {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            GameView.RemoveAlert(e.Notification);
        }
    }
}
