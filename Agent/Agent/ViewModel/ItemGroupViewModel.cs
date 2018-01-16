using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.ViewModel;
using Agent.ViewModel.Util;
using Core.Model.Filter;

namespace Agent.ViewModel
{
    public class ItemGroupViewModel : VM, IUpdatable
    {
        public ItemGroupViewModel(string name) => Name = name;

        public string Name { get; }

        BatchedObservableCollection<ExtendedItemViewModel> items =
            new BatchedObservableCollection<ExtendedItemViewModel>();

        public ObservableCollection<ExtendedItemViewModel> Items => items;

        public void AddRange(IEnumerable<ExtendedItemViewModel> newItems) =>
            items.AddRange(newItems);

        public ExtendedItemViewModel TryGetItem(Item item) => items.SingleOrDefault(it => it.Original.Item == item);
        public void RemoveRangeByModel(IEnumerable<Item> modelItems)
        {
            var hash = new HashSet<Item>(modelItems);
            var seq = items.Where(it => hash.Contains(it.Original.Item));
            items.RemoveAll(seq);
        }

        public void Update() { }
    }
}
