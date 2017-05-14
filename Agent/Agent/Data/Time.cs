using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Agent.Data
{
    class Time : INotifyPropertyChanged
    {
        public Time()
        {
            var timer = new Timer {Interval = 5000};
            // 5 second updates
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public long Now => (long) Tools.Time.ToUnixTime(DateTime.Now);

        public event PropertyChangedEventHandler PropertyChanged;

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
        }
    }
}
