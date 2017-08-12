using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Model;

namespace Core.ViewModel
{
    public class SectorViewModel
    {
        private SectorViewModel(string planet, string location)
        {
            Planet = planet;
            Location = location;
        }

        public static SectorViewModel FromSector(string sector)
        {
            var filtered = sector.GetFilter(Model.Filters.FilterType.Planet).FirstOrDefault().Key;
            string planet = null, location = null;
            if (filtered != null)
            {
                var parts = filtered.ToUpper().Split('|');
                if (parts.Length == 2)
                {
                    planet = parts[0];
                    location = parts[1];
                }
            }
            return new SectorViewModel(planet, location ?? sector);
        }

        public string Planet { get; }
        public string Location { get; }
    }
}
