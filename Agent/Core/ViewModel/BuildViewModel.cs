using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Name = $"Строение {build.Number}";
            Value = build.Value;
            Faction = FactionViewModel.ById("?");
        }
    }
}
