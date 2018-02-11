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
            Id = item.Id;
            Item = item;
            Update();
        }

        public ItemViewModel(string id)
        {
            Id = id;
            Update();
        }

        public string Id { get; }

        private Model.Filter.Item _item;
        public Model.Filter.Item Item
        {
            get => _item;
            private set => Set(ref _item, value);
        }

        private string _value;
        public string Value
        {
            get => _value;
            private set => Set(ref _value, value);
        }

        private string _type;
        public string Type
        {
            get => _type;
            private set => Set(ref _type, value);
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            private set => Set(ref _enabled, value);
        }

        public Brush _color;
        public Brush RewardColor
        {
            get => _color;
            private set => Set(ref _color, value);
        }

        public void Update()
        {
            if (Item == null)
                Item = Model.Filters.ExpandItem(Id);
            if (Item != null)
            {
                Value = Item.Value ?? Id;
                Type = Item.Type;
                Enabled = Item.Enabled;
            }
            else
            {
                Value = Id;
                Type = null;
                Enabled = false;
            }
            RewardColor = GetBrushForReward(Type);
        }

        static Brush GetBrushForReward(string rewardType)
        {
            switch (rewardType) //TODO: Переделать на универсальный тип (если будет смена языка, то названия не будут на русском).
            {
            case "Шлема":
                return Brushes.YellowGreen;
            case "Чертежи":
                return Brushes.BlueViolet;
            case "Ауры":
                return Brushes.OrangeRed;
            case "Модификаторы":
                return Brushes.DarkCyan;
            default:
                return null;
            }
        }
    }
}
