using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Core.Events;
using Core.Model;

namespace Core.ViewModel
{
    public class DailyDealViewModel : VM
    {
        private DailyDeal dailyDeal;

        public DailyDealViewModel(DailyDeal dailyDeal, FiltersEvent filtersEvent)
        {
            this.dailyDeal = dailyDeal;

            Activation = Tools.Time.ToDateTime(dailyDeal.Activation.Date.NumberLong);
            Expiry = Tools.Time.ToDateTime(dailyDeal.Expiry.Date.NumberLong);
            StoreItem = Model.Filters.ExpandItem(dailyDeal.StoreItem)?.Value ?? dailyDeal.StoreItem;
            StoreItemOriginal = dailyDeal.StoreItem;
            Discount = dailyDeal.Discount;
            OriginalPrice = dailyDeal.OriginalPrice;
            SalePrice = dailyDeal.SalePrice;
            AmountTotal = dailyDeal.AmountTotal;
            AmountSold = dailyDeal.AmountSold;
            Update();
            ItemsUpdatedWeakEventManager.AddHandler(filtersEvent, OnItemsFilterUpdated);
        }

        void OnItemsFilterUpdated(object sender, EventArgs args) => Update();


        public void Update()
        {
            AmountSold = dailyDeal.AmountSold;
        }

        public string StoreItemOriginal { get; set; }
        public string StoreItem { get; set; }
        public DateTime Activation { get; }
        public DateTime Expiry { get; }
        public int Discount { get; }
        public int OriginalPrice { get; }
        public int SalePrice { get; }
        public int AmountTotal { get; }

        private int _ammountSold;

        public int AmountSold
        {
            get => _ammountSold;
            set => Set(ref _ammountSold, value);
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
            if (DateTime.Now <= Expiry)
            {
                Status = (Expiry - DateTime.Now).ToString((Expiry - DateTime.Now.TimeOfDay).Hour == 0
                    ? @"mm\:ss"
                    : @"hh\:mm\:ss");
                StatusColor = new SolidColorBrush(Color.FromRgb(r: 0x37, g: 0x82, b: 0xCD));

                if ((Expiry - DateTime.Now).Hours < 1)
                    StatusColor = Brushes.LightSlateGray;
            }
            else
            {
                Status = "00:00";
                StatusColor = Brushes.Red;
            }
        }

    }
}
