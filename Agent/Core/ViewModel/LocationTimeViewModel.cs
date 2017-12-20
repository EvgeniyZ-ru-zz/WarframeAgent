using System;
using System.Windows.Threading;

namespace Core.ViewModel
{
    public class LocationTimeViewModel : VM
    {
        DateTime time = new DateTime(2017, 12, 20);
        public DateTime Time
        {
            get => time;
            set => Set(ref time, value);
        }

        public string LocationName { get; } = "Земля";
    }
}
