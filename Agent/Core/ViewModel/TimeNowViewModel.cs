using System;
using System.Windows.Threading;

namespace Core.ViewModel
{
    public class TimeNowViewModel : VM
    {
        public TimeNowViewModel()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += (o, args) => NotifyPropertyChanged(nameof(Now));
            timer.Start();
        }

        public DateTime Now => DateTime.Now;
    }
}
