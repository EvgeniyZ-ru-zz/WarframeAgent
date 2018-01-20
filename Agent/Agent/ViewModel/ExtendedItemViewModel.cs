using System;
using System.Windows.Input;
using Core.ViewModel;

namespace Agent.ViewModel
{
    public class ExtendedItemViewModel : VM
    {
        internal ExtendedItemViewModel(
            ItemViewModel item, bool isNotificationEnabled, Action<ExtendedItemViewModel> notificationEnabledCallback)
        {
            Original = item;
            this.isNotificationEnabled = isNotificationEnabled;
            this.notificationEnabledCallback = notificationEnabledCallback;
            ToggleNotification = new RelayCommand(() => IsNotificationEnabled = !IsNotificationEnabled);
        }

        public ItemViewModel Original { get; }

        readonly Action<ExtendedItemViewModel> notificationEnabledCallback;

        bool isNotificationEnabled;
        public bool IsNotificationEnabled
        {
            get => isNotificationEnabled;
            set { if (Set(ref isNotificationEnabled, value)) notificationEnabledCallback(this); }
        }

        public ICommand ToggleNotification { get; }

        public void Update() => Original.Update();
    }
}
