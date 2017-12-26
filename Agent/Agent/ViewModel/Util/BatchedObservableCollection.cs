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

        // это грязный хак, который подсмотрен здесь:
        // http://geekswithblogs.net/NewThingsILearned/archive/2008/01/16/listcollectionviewcollectionview-doesnt-support-notifycollectionchanged-with-multiple-items.aspx
        // он обходит проблему с контролом CollectionView, который не умеет обрабатывать
        // неодноэлементные изменения
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handlers = CollectionChanged;
            if (handlers != null)
            {
                using (BlockReentrancy())
                {
                    foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
                    {
                        if (handler.Target is System.Windows.Data.CollectionView cv)
                            cv.Refresh();
                        else
                            handler(this, e);
                    }
                }
            }
        }

        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
    }
}
