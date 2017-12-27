using System.Windows.Media;

namespace Core.ViewModel
{
    public class FactionViewModel : VM
    {
        string name;
        Brush color;
        Geometry logo;
        public string Name { get => name; private set => Set(ref name, value); }
        public Brush Color { get => color; private set => Set(ref color, value); }
        public Geometry Logo { get => logo; private set => Set(ref logo, value); }

        public static FactionViewModel ById(string factionId) => FactionsEngine.ById(factionId);

        internal void UpdateTo(string name, Brush color, Geometry logo)
        {
            // присвоение свойствам, чтобы сработала нотификация об изменениях
            Name = name;
            Color = color;
            Logo = logo;
        }

        internal FactionViewModel(string name, Brush color, Geometry logo)
        {
            this.name = name;
            this.color = color;
            this.logo = logo;
        }
    }
}
