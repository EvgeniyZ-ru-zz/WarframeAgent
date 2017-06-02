using System;
using System.Windows.Threading;

namespace Core.ViewModel
{
    public class TimeNowViewModel : VM
    {
        public TimeNowViewModel()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += Timer_Elapsed;
            timer.Start();
        }

        private long _now = Tools.Time.ToUnixTime(DateTime.Now);

        public long Now
        {
            get => _now;
            set => Set(ref _now, value);
        } 

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            Now = Tools.Time.ToUnixTime(DateTime.Now);
        }
    }
}
