using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Core.ViewModel;

namespace Agent.ViewModel
{
    public class ExtendedItemViewModel : ItemViewModel
    {
        internal ExtendedItemViewModel(
            Core.Model.Filter.Item item, IReadOnlyDictionary<NotificationTarget, SubscriptionState> notificationState) : base(item)
        {
            NotificationState = notificationState;
            ToggleNotification = new RelayCommand<NotificationTarget>(target =>
                { var state = NotificationState[target]; state.NotificationEnabled = !state.NotificationEnabled; });
        }

        internal ExtendedItemViewModel(
            string id, IReadOnlyDictionary<NotificationTarget, SubscriptionState> notificationState) : base(id)
        {
            NotificationState = notificationState;
            ToggleNotification = new RelayCommand<NotificationTarget>(target =>
                { var state = NotificationState[target]; state.NotificationEnabled = !state.NotificationEnabled; });
        }

        public IReadOnlyDictionary<NotificationTarget, SubscriptionState> NotificationState { get; }
        public ICommand ToggleNotification { get; }
    }

    // нам нужна обёртка на bool, чтобы не требовалось двустронней мультипривязки
    public class SubscriptionState : VM
    {
        bool notificationEnabled;
        public bool NotificationEnabled
        {
            get => notificationEnabled;
            set => Set(ref notificationEnabled, value);
        }
    }
}
