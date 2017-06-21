using Core.ViewModel;

namespace Core.Model
{
    public class Item : VM
    {
        public int Id { get; set; }

        private string _type;

        public string Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _value;

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
}
