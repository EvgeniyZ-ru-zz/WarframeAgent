using System;
using System.Windows.Media;

using Core.Model;
using Core.Events;

namespace Core.ViewModel
{
    public class ItemViewModel : VM, IUpdatable
    {
        public ItemViewModel(Model.Filter.Item item)
        {
            Item = item;
            Update();
        }

        public Model.Filter.Item Item { get; }

        private string _value;
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        private string _type;
        public string Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => Set(ref _enabled, value);
        }

        public void Update() =>
            (Value, Type, Enabled) = (Item.Value, Item.Type, Item.Enabled);
    }
}
