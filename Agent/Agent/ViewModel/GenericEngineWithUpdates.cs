using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Events;
using Core.Model;
using Core.ViewModel;

namespace Agent.ViewModel
{
    abstract class GenericEngineWithUpdates<ItemVM, ItemModel> : GenericSimpleEngine<ItemVM, ItemModel> where ItemVM : VM, IUpdatable
    {
        public GenericEngineWithUpdates(FiltersEvent filtersEvent) : base(filtersEvent) { }

        protected abstract void LogChanged(ItemModel item);

        protected async void ChangeEvent(object sender, NotificationEventArgs<ItemModel> e)
        {
            await AsyncHelpers.RedirectToMainThread();
            ChangeEventImpl(e.Notifications);
        }

        protected virtual void ChangeEventImpl(IReadOnlyCollection<ItemModel> changedItems)
        {
            foreach (var item in changedItems)
            {
                LogChanged(item);

                var buildVM = TryGetItemByModel(item);
                buildVM?.Update();
            }
        }
    }
}
