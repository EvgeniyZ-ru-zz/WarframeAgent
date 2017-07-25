using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Core.Model;
using Newtonsoft.Json.Linq;

namespace Core.ViewModel
{
    public static class Filters
    {
        public enum FilterType
        {
            Item,
            Planet,
            Mission,
            Fraction
        }

        public static Dictionary<string, string> GetFilter(this string value, FilterType type)
        {
            switch (type)
            {
                case FilterType.Item:
                    return FiltersViewModel.Items.Find(value, "Items");
                case FilterType.Fraction:
                    return FiltersViewModel.Races.Find(value, "Items");
                case FilterType.Planet:
                    return FiltersViewModel.Planets.Find(value, "Items");
                case FilterType.Mission:
                    return FiltersViewModel.Missions.Find(value, "Missions");
                default:
                    return new Dictionary<string, string> { { value, null } };
            }
        }
    }

    public class FiltersViewModel
    {
        private static Dictionary<string, string> ReadFile(string file, string value, string cat)
        {
            try
            {
                var strings = File.ReadAllText(file, Encoding.UTF8);
                var json = JObject.Parse(strings);
                var result = json[cat]
                    .Where(s => s[value] != null)
                    .Select(s => new
                    {
                        Value = s[value]?.ToString(),
                        Type = s["type"]?.ToString()
                    }).ToDictionary(p => p.Value, e => e.Type);

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new Dictionary<string, string> { { value, null } };
            }
        }

        public static class Items
        {
            public static Dictionary<string, string> Find(string value, string cat)
            {
                return ReadFile("Filters/Items.json", value, cat);
            }
        }

        public static class Races
        {
            public static Dictionary<string, string> Find(string value, string cat)
            {
                return ReadFile("Filters/Race.json", value, cat);
            }
        }

        public static class Planets
        {
            public static Dictionary<string, string> Find(string value, string cat)
            {
                return ReadFile("Filters/Planets.json", value, cat);
            }
        }

        public static class Missions
        {
            public static Dictionary<string, string> Find(string value, string cat)
            {
                return ReadFile("Filters/Missions.json", value, cat);
            }
        }
    }
}
