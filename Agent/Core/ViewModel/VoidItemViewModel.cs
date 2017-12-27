using System;
using Core.Events;
using Core.Model;

namespace Core.ViewModel
{
    public class VoidItemViewModel
    {
        private Manifest manifest;
        public VoidItemViewModel(Manifest manifest, FiltersEvent filtersEvent)
        {
            ItemType = Model.Filters.ExpandItem(manifest.ItemType)?.Value ?? manifest.ItemType;
            PrimePrice = manifest.PrimePrice;
            RegularPrice = manifest.RegularPrice;

            this.manifest = manifest;

            ItemsUpdatedWeakEventManager.AddHandler(filtersEvent, OnItemsFilterUpdated);
        }

        private void OnItemsFilterUpdated(object sender, EventArgs eventArgs)
        {
            ItemType = Model.Filters.ExpandItem(manifest.ItemType)?.Value ?? manifest.ItemType;
        }

        public string ItemType { get; set; }
        public int PrimePrice { get; set; }
        public int RegularPrice { get; set; }
    }
}
