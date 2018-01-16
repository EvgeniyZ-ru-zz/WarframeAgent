using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core;
using Core.Events;
using Core.Model;
using Core.ViewModel;
using NLog;

namespace Agent.ViewModel
{
    abstract class GenericEngineWithUpdates<ItemVM, ItemModel> : GenericSimpleEngine<ItemVM, ItemModel> where ItemVM : VM, IUpdatable
    {
        public GenericEngineWithUpdates(FiltersEvent filtersEvent) : base(filtersEvent) { }

        protected abstract string LogChangedOne(ItemModel item);
        protected abstract string LogChangedMany(int n);

        protected virtual void LogChanged(IReadOnlyCollection<ItemModel> changedItems)
        {
            var message = (changedItems.Count == 1) ? LogChangedOne(changedItems.First()) : LogChangedMany(changedItems.Count);
            Tools.Logging.Send(LogLevel.Debug, message);
        }

        protected async void ChangeEvent(object sender, NotificationEventArgs<ItemModel> e)
        {
            await AsyncHelpers.RedirectToMainThread();
            ChangeEventImpl(e.Notifications);
        }

        protected virtual void ChangeEventImpl(IReadOnlyCollection<ItemModel> changedItems)
        {
            LogChanged(changedItems);
            foreach (var item in changedItems)
                TryGetItemByModel(item)?.Update();
        }
    }
}
