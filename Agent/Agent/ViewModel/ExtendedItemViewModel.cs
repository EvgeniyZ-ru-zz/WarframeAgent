using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Core.ViewModel;

namespace Agent.ViewModel
{
    public class ExtendedItemViewModel : VM
    {
        internal ExtendedItemViewModel(
            ItemViewModel item, IReadOnlyDictionary<NotificationTarget, SubscriptionState> notificationState)
        {
            Original = item;
            NotificationState = notificationState;
            ToggleNotification = new RelayCommand<NotificationTarget>(target =>
                { var state = NotificationState[target]; state.NotificationEnabled = !state.NotificationEnabled; });
        }

        public ItemViewModel Original { get; }

        public IReadOnlyDictionary<NotificationTarget, SubscriptionState> NotificationState { get; }
        public ICommand ToggleNotification { get; }

        public void Update() => Original.Update();
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
