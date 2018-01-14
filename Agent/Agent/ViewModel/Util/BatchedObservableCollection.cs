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
            InsertRange(seq, -1);

        public void InsertRange(IEnumerable<T> seq, int index)
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            CheckReentrancy();
            var items = Items;
            var newItems = seq.ToList();
            if (newItems.Count == 0)
                return;
            if (index == -1)
                index = Count;
            var curr = index;
            foreach (var el in newItems)
                items.Insert(curr++, el);

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
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            CheckReentrancy();
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

        public void ReplaceRange(List<T> remSeq, IEnumerable<T> addSeq)
        {
            if (remSeq == null)
                throw new ArgumentNullException(nameof(remSeq));
            if (addSeq == null)
                throw new ArgumentNullException(nameof(addSeq));
            CheckReentrancy();
            int idxToAdd = -1;
            var items = Items;
            var removedItems = new List<T>();
            foreach (var el in remSeq)
            {
                int index = items.IndexOf(el);
                if (index < 0) continue;
                if (idxToAdd < 0) idxToAdd = index;
                removedItems.Add(el);
                items.RemoveAt(index);
            }

            if (idxToAdd < 0) idxToAdd = Count;
            var indexSave = idxToAdd;

            var newItems = addSeq.ToList();
            foreach (var el in newItems)
                items.Insert(idxToAdd++, el);

            if (removedItems.Count == 0 && newItems.Count == 0)
                return;

            if (removedItems.Count != newItems.Count)
                OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    newItems,
                    removedItems,
                    indexSave));
        }

        public void MoveRange(List<T> itemsToMove, int newIndex)
        {
            if (itemsToMove == null)
                throw new ArgumentNullException(nameof(itemsToMove));
            CheckReentrancy();
            var items = Items;
            var temporaryItems = new List<T>();
            foreach (var el in itemsToMove)
            {
                int index = items.IndexOf(el);
                if (index < 0) continue;
                temporaryItems.Add(el);
                items.RemoveAt(index);
            }

            if (temporaryItems.Count == 0)
                return;
            if (newIndex == -1)
                newIndex = Count;
            var currentIndex = newIndex;
            foreach (var el in temporaryItems)
                items.Insert(currentIndex++, el);

            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Move,
                    temporaryItems,
                    newIndex));
        }

        public void Reset(IEnumerable<T> seq)
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            CheckReentrancy();
            var items = Items;
            var oldCount = items.Count;
            items.Clear();
            foreach (var el in seq)
                items.Add(el);

            if (oldCount != items.Count)
                OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
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
