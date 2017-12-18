using Core.Model;

namespace Core.ViewModel
{
    public class BuildViewModel : VM
    {
        string name;
        double val;
        FactionViewModel faction;

        public int Id { get; }
        public string Name { get => name; set => Set(ref name, value); }
        public double Value { get => val; set => Set(ref val, value); }
        public FactionViewModel Faction { get => faction; set => Set(ref faction, value); }

        private Build build;

        public BuildViewModel(Build build)
        {
            this.build = build;
            Id = build.Number;
            Update();
        }

        public void Update()
        {
            var filteredBuild = Model.Filters.ExpandBuild(Id);
            Name = filteredBuild?.Name ?? $"Строение {Id}";
            Faction = FactionViewModel.ById(filteredBuild?.Faction);
            Value = build.Value;
        }
    }
}
