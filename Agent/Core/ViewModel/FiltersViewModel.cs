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
    public class ItemComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item x, Item y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Item obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public static class Filters
    {
        public enum FilterType
        {
            Item,
            Planet,
            Mission,
            Fraction
        }

        public static string GetFilter(this string value, FilterType type)
        {
            switch (type)
            {
                case FilterType.Item:
                    return FiltersViewModel.Items.Find(value);
                case FilterType.Fraction:
                    return FiltersViewModel.Races.Find(value);
                case FilterType.Planet:
                    return FiltersViewModel.Planets.Find(value);
                case FilterType.Mission:
                    return FiltersViewModel.Missions.Find(value);
                default: return value;
            }
        }
    }

    public class FiltersViewModel
    {
        private static object ReadFile(string file, string value)
        {
            try
            {
                var strings = File.ReadAllLines(file, Encoding.UTF8);
                var p = "{" + strings.Where(x => x.Contains(value)).Select(x => x).ToArray()[0] + "}";
                var obj = JObject.Parse(p);
                return (string)obj[value];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return value;
            }
        }

        public static class Items
        {
            public static string Find(string value)
            {
                return ReadFile("Filters/Items.json", value).ToString();
            }
        }

        public static class Races
        {
            public static string Find(string value)
            {
                return ReadFile("Filters/Race.json", value).ToString();
            }
        }

        public static class Planets
        {
            public static string Find(string value)
            {
                return ReadFile("Filters/Planets.json", value).ToString();
            }
        }

        public static class Missions
        {
            public static string Find(string value)
            {
                return ReadFile("Filters/Missions.json", value).ToString();
            }
        }

    }
}
