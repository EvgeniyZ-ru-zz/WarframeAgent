using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Core.Model;

namespace Core.ViewModel
{
    public class AlertViewModel : VM
    {
        public AlertViewModel(Id id, DateTime activation, DateTime expiry, MissionViewModel mission)
        {
            Id = id;
            Activation = activation;
            Expiry = expiry;
            MissionInfo = mission;
            UpdateStatus();
        }

        public Id Id { get; }
        public DateTime Activation { get; }
        public DateTime Expiry { get; }
        public MissionViewModel MissionInfo { get; }

        private Brush _statusColor;
        public Brush StatusColor
        {
            get => _statusColor;
            set => Set(ref _statusColor, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public void UpdateStatus()
        {
            if (Activation >= DateTime.Now)
            {
                Status = (Activation - DateTime.Now).ToString(@"mm\:ss");
                StatusColor = Brushes.Orange;
            }
            else
            {
                if (DateTime.Now <= Expiry)
                {
                    Status = (Expiry - DateTime.Now).ToString((Expiry - DateTime.Now.TimeOfDay).Hour == 0
                        ? @"mm\:ss"
                        : @"hh\:mm\:ss");
                    StatusColor = new SolidColorBrush(Color.FromRgb(r: 0x6E, g: 0xCD, b: 0x37));
                }
                else
                {
                    Status = "00:00";
                    StatusColor = Brushes.Red;
                }
            }
        }
    }
}
