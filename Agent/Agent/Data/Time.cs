using Core;
using System;
using System.Timers;
using System.Windows.Threading;
using Core.ViewModel;

namespace Agent.Data
{
    internal class Time : VM
    {
        public Time()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += timer_Elapsed;
            timer.Start();
        }

        private long _now = Tools.Time.ToUnixTime(DateTime.Now);

        public long Now
        {
            get => _now;
            set => Set(ref _now, value);
        } 

        private void timer_Elapsed(object sender, EventArgs e)
        {
            Now = Tools.Time.ToUnixTime(DateTime.Now);
        }
    }
}
