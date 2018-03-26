using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agent.ViewModel;
using Agent.View;
using Core.ViewModel;

namespace Agent
{
    public class ToastManager
    {
        ObservableCollection<ToastViewModel> toasts = new ObservableCollection<ToastViewModel>();

        HashSet<ToastViewModel> toastsOnHold = new HashSet<ToastViewModel>();
        HashSet<ToastViewModel> zombies = new HashSet<ToastViewModel>(); // on hold and removed

        ToastWindow window;

        public void AddToast(ToastViewModel toast)
        {
            zombies.Remove(toast);
            if (!toasts.Contains(toast))
                toasts.Add(toast);
            if (toasts.Count > 0)
                OpenToastWindow();
        }

        public async void AddToastWithTimeout(ToastViewModel toast, TimeSpan timeout)
        {
            AddToast(toast);
            await Task.Delay(timeout);
            RemoveToast(toast);
        }

        public void RemoveToast(ToastViewModel toast)
        {
            if (toastsOnHold.Contains(toast))
                zombies.Add(toast);
            else
                RemoveToastFromToasts(toast);
        }

        void RemoveToastFromToasts(ToastViewModel toast)
        {
            if (toasts.Count == 1)
            {
                CloseToastWindow();
                toasts = new ObservableCollection<ToastViewModel>();
            }
            else
            {
                toasts.Remove(toast);
            }
        }

        public void HoldToast(ToastViewModel toast)
        {
            toastsOnHold.Add(toast);
        }

        public void UnholdToast(ToastViewModel toast)
        {
            toastsOnHold.Remove(toast);
            if (zombies.Remove(toast))
                RemoveToastFromToasts(toast);
        }

        void OpenToastWindow()
        {
            window = new ToastWindow()
            {
                DataContext = new ReadOnlyObservableCollection<ToastViewModel>(toasts),
                HoldCommand = new RelayCommand<ToastViewModel>(HoldToast),
                UnholdCommand = new RelayCommand<ToastViewModel>(UnholdToast)
            };
            window.Show();
        }

        void CloseToastWindow()
        {
            window.Close();
            window = null;
        }
    }
}
