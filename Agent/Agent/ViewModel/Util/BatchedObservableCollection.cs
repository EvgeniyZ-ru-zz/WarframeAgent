using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.ViewModel.Util
{
    class BatchedObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> seq) =>
            InsertRange(seq, Count);

        public void InsertRange(IEnumerable<T> seq, int index)
        {
            CheckReentrancy();
            var items = Items;
            var newItems = seq.ToList();
            if (newItems.Count == 0)
                return;
            foreach (var el in newItems)
                items.Add(el);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    newItems,
                    index));
        }

        public void RemoveAll(IEnumerable<T> seq)
        {
            var items = Items;
            var removedItems = new List<T>();
            foreach (var el in seq)
            {
                int index = items.IndexOf(el);
                if (index < 0) continue;
                removedItems.Add(el);
                items.RemoveAt(index);
            }
            if (removedItems.Count == 0)
                return;

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    removedItems));
        }

        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
    }
}
