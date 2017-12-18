using Core.Model;

namespace Core.ViewModel
{
    public class VoidItemViewModel
    {
        public VoidItemViewModel(Manifest manifest)
        {
            ItemType = Model.Filters.ExpandItem(manifest.ItemType)?.Value ?? manifest.ItemType;
            PrimePrice = manifest.PrimePrice;
            RegularPrice = manifest.RegularPrice;
        }

        public string ItemType { get; set; }
        public int PrimePrice { get; set; }
        public int RegularPrice { get; set; }
    }
}
