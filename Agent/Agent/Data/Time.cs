using Core;
using System;
using System.Timers;

namespace Agent.Data
{
    internal class Time : VM
    {
        public Time()
        {
            var timer = new Timer {Interval = 5000};
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private long _now = Tools.Time.ToUnixTime(DateTime.Now);

        public long Now
        {
            get => _now;
            set => Set(ref _now, value);
        } 

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Now = Tools.Time.ToUnixTime(DateTime.Now);
        }
    }
}
