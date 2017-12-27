using System;
using System.Windows.Media;
using Core.Events;
using Core.Model;

namespace Core.ViewModel
{
    public class VoidTradeViewModel : VM
    {
        private VoidTrader trader;

        public VoidTradeViewModel(VoidTrader trader, FiltersEvent filtersEvent)
        {
            Id = trader.Id;
            Activation = Tools.Time.ToDateTime(trader.Activation.Date.NumberLong);
            PreActivation = Activation.AddDays(-12);
            Expiry = Tools.Time.ToDateTime(trader.Expiry.Date.NumberLong);
            Character = trader.Character == "Baro'Ki Teel" ? "Баро Ки'Тиир" : trader.Character; //TODO: Перевод
            Location = Model.Filters.ExpandSector(trader.Node)?.Location ?? trader.Node;
            Planet = Model.Filters.ExpandSector(trader.Node)?.Planet ?? trader.Node;

            this.trader = trader;

            PlanetsUpdatedWeakEventManager.AddHandler(filtersEvent, OnPlanetsFilterUpdated);
        }

        private void OnPlanetsFilterUpdated(object sender, EventArgs eventArgs)
        {
            Location = Model.Filters.ExpandSector(trader.Node)?.Location ?? trader.Node;
            Planet = Model.Filters.ExpandSector(trader.Node)?.Planet ?? trader.Node;
        }

        public Id Id { get; }
        public DateTime PreActivation { get; }
        public DateTime Activation { get; }
        public DateTime Expiry { get; }
        public string Character { get; }

        private string location;
        public string Location { get => location; set=> Set(ref location, value); }

        private string planet;
        public string Planet { get => planet; set => Set(ref planet, value); }

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
                StatusText = "Отсутствует".ToUpper();
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
                    Status = "--:--";
                    StatusText = "Улетает".ToUpper();
                    StatusColor = Brushes.Red;
                }
            }
        }
    }
}
