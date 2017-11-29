using System;
using System.Windows.Media;

using Core.Model;

namespace Core.ViewModel
{
    public class VoidTradeViewModel : VM
    {
        public VoidTradeViewModel(VoidTrader trader)
        {
            Id = trader.Id;
            Activation = Tools.Time.ToDateTime(trader.Activation.Date.NumberLong);
            PreActivation = Activation.AddDays(-12);
            Expiry = Tools.Time.ToDateTime(trader.Expiry.Date.NumberLong);
            Character = trader.Character == "Baro'Ki Teel" ? "Баро Ки'Тиир" : trader.Character; //TODO: Перевод
            Location = Model.Filters.ExpandSector(trader.Node).Location;
            Planet = Model.Filters.ExpandSector(trader.Node).Planet;
        }

        public Id Id { get; }
        public DateTime PreActivation { get; }
        public DateTime Activation { get; }
        public DateTime Expiry { get; }
        public string Character { get; }
        public string Location { get; }
        public string Planet { get; }

        private string _statusText;

        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }
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
            if (DateTime.Now <= Activation)
            {
                var stat = Activation - DateTime.Now;
                Status = stat.ToString(@"dd\:hh\:mm\:ss");
                StatusText = "Отсутсвует".ToUpper();
                StatusColor = new SolidColorBrush(Color.FromRgb(r: 0x37, g: 0x82, b: 0xCD));
            }
            else
            {
                if (DateTime.Now <= Expiry)
                {
                    Status = (Expiry - DateTime.Now).ToString((Expiry - DateTime.Now.TimeOfDay).Hour == 0
                        ? @"mm\:ss"
                        : @"hh\:mm\:ss");
                    StatusText = "Прилетел".ToUpper();
                    StatusColor = new SolidColorBrush(Color.FromRgb(r: 0x6E, g: 0xCD, b: 0x37));
                }
                else
                {
                    Status = "00:00";
                    StatusText = "Улетает".ToUpper();
                    StatusColor = Brushes.Red;
                }
            }
        }
    }
}
