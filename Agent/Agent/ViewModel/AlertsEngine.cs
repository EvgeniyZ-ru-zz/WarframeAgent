﻿using System;
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

        public void Run(NotificationModel watcher)
        {
            watcher.AlertNotificationArrived += AddEvent;
            watcher.AlertNotificationDeparted += RemoveEvent;
        }

        private async void AddEvent(object sender, NewAlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();

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
                    e.Notification.MissionInfo.RewardColor = (Brush)Application.Current.Resources["TextColor"];
                    break;
            }

            #endregion

            e.Notification.MissionInfo.Faction = e.Notification.MissionInfo.Faction.GetFilter(Filters.FilterType.Fraction).FirstOrDefault().Key;
            e.Notification.MissionInfo.Planet = e.Notification.MissionInfo.Location.GetFilter(Filters.FilterType.Planet).FirstOrDefault().Key.ToUpper().Split('|');
            e.Notification.MissionInfo.MissionType = e.Notification.MissionInfo.MissionType.GetFilter(Filters.FilterType.Mission).FirstOrDefault().Key;

            GameView.AddAlert(e.Notification);
        }

        private async void RemoveEvent(object sender, RemovedAlertNotificationEventArgs e)
        {
            await AsyncHelpers.RedirectToMainThread();
            Debug.WriteLine($"Удаляю тревогу {e.Notification.Id.Oid}!", $"[{DateTime.Now}]");

            GameView.RemoveAlert(e.Notification);
        }
    }
}