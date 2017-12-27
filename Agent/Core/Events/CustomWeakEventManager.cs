using System;
using System.Windows;

namespace Core.Events
{
    abstract class CustomWeakEventManager<TSelf, TArgs> : WeakEventManager
        where TSelf : CustomWeakEventManager<TSelf, TArgs>
        where TArgs : EventArgs
    {
        protected CustomWeakEventManager() { }

        public static void AddHandler(FiltersEvent source, EventHandler<TArgs> handler) =>
            CurrentManager.ProtectedAddHandler(
                source ?? throw new ArgumentNullException(nameof(source)),
                handler ?? throw new ArgumentNullException(nameof(handler)));

        public static void RemoveHandler(FiltersEvent source, EventHandler<TArgs> handler) =>
            CurrentManager.ProtectedRemoveHandler(
                source ?? throw new ArgumentNullException(nameof(source)),
                handler ?? throw new ArgumentNullException(nameof(handler)));

        private static TSelf CurrentManager
        {
            get
            {
                var managerType = typeof(TSelf);
                var manager = (TSelf)GetCurrentManager(managerType);

                // at first use, create and register a new manager
                if (manager == null)
                {
                    manager = (TSelf)Activator.CreateInstance(managerType, nonPublic: true);
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        protected override ListenerList NewListenerList() =>
            new ListenerList<TArgs>();

        protected void OnItemsUpdated(object sender, TArgs e) =>
            DeliverEvent(sender, e);
    }

    #region Items 

    class ItemsUpdatedWeakEventManager : CustomWeakEventManager<ItemsUpdatedWeakEventManager, EventArgs>
    {
        private ItemsUpdatedWeakEventManager() { }

        protected override void StartListening(object source) =>
            ((FiltersEvent)source).ItemsUpdated += OnItemsUpdated;

        protected override void StopListening(object source) =>
            ((FiltersEvent)source).ItemsUpdated -= OnItemsUpdated;
    }

    #endregion

    #region Sectors 

    class SectorsUpdatedWeakEventManager : CustomWeakEventManager<SectorsUpdatedWeakEventManager, EventArgs>
    {
        private SectorsUpdatedWeakEventManager() { }

        protected override void StartListening(object source) =>
            ((FiltersEvent)source).SectorsUpdated += OnItemsUpdated;

        protected override void StopListening(object source) =>
            ((FiltersEvent)source).SectorsUpdated -= OnItemsUpdated;
    }

    #endregion

    #region Missions 

    class MissionsUpdatedWeakEventManager : CustomWeakEventManager<MissionsUpdatedWeakEventManager, EventArgs>
    {
        private MissionsUpdatedWeakEventManager() { }

        protected override void StartListening(object source) =>
            ((FiltersEvent)source).MissionsUpdated += OnItemsUpdated;

        protected override void StopListening(object source) =>
            ((FiltersEvent)source).MissionsUpdated -= OnItemsUpdated;
    }

    #endregion

    #region Factions 

    class FactionsUpdatedWeakEventManager : CustomWeakEventManager<FactionsUpdatedWeakEventManager, EventArgs>
    {
        private FactionsUpdatedWeakEventManager() { }

        protected override void StartListening(object source) =>
            ((FiltersEvent)source).FactionsUpdated += OnItemsUpdated;

        protected override void StopListening(object source) =>
            ((FiltersEvent)source).FactionsUpdated -= OnItemsUpdated;
    }

    #endregion

    #region Builds 

    class BuildsUpdatedWeakEventManager : CustomWeakEventManager<BuildsUpdatedWeakEventManager, EventArgs>
    {
        private BuildsUpdatedWeakEventManager() { }

        protected override void StartListening(object source) =>
            ((FiltersEvent)source).BuildsUpdated += OnItemsUpdated;

        protected override void StopListening(object source) =>
            ((FiltersEvent)source).BuildsUpdated -= OnItemsUpdated;
    }

    #endregion

    #region Planets 

    class PlanetsUpdatedWeakEventManager : CustomWeakEventManager<PlanetsUpdatedWeakEventManager, EventArgs>
    {
        private PlanetsUpdatedWeakEventManager() { }

        protected override void StartListening(object source) =>
            ((FiltersEvent)source).PlanetsUpdated += OnItemsUpdated;

        protected override void StopListening(object source) =>
            ((FiltersEvent)source).PlanetsUpdated -= OnItemsUpdated;
    }

    #endregion

}
