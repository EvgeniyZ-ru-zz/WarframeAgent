using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Core.Model
{
    using Filter;

    namespace Filter
    {
        public enum Type
        {
            Builds,
            Factions,
            Items,
            Missions,
            Planets,
            Race,
            Sorties,
            Void
        }

        public class Build
        {
            public Build(string name, string faction) { Name = name; Faction = faction; }
            public string Name { get; }
            public string Faction { get; }
        }

        public class Faction
        {
            public Faction(string name, string color, string logo) { Name = name; Color = color; Logo = logo; }
            public string Name { get; }
            public string Color { get; }
            public string Logo { get; }
        }

        public class Item
        {
            public Item(string value, string type, bool enabled) { Value = value; Type = type; Enabled = enabled; }
            public string Value { get; }
            public string Type { get; }
            public bool Enabled { get; }
        }

        public class Sector
        {
            public Sector(string planet, string location) { Planet = planet; Location = location; }
            public string Planet { get; }
            public string Location { get; }
        }

        public class Mission
        {
            public Mission(string name) { Name = name; }
            public string Name { get; }
        }
    }

    public static class Filters
    {
        static T Expand<K, T>(K item, Dictionary<K, T> dict, Type type) where T : class
        {
            if (item == null)
                return null;
            if (dict == null)
                return null;
            bool isFilterFood = dict.TryGetValue(item, out var result);
            if (!isFilterFood)
                BadFilterReportModel.ReportBadFilter(item.ToString(), type);
            return result;
        }

        public static Item ExpandItem(string item) => Expand(item, FiltersModel.AllItems, Type.Items);
        public static Sector ExpandSector(string item) => Expand(item, FiltersModel.AllSectors, Type.Planets);
        public static Mission ExpandMission(string item) => Expand(item, FiltersModel.AllMissions, Type.Missions);
        public static Faction ExpandFaction(string item) => Expand(item, FiltersModel.AllFactions, Type.Factions);
        public static Filter.Build ExpandBuild(int item) => Expand(item, FiltersModel.AllBuilds, Type.Builds);
    }

    class FiltersModel
    {
        internal static Dictionary<string, Item> AllItems;
        internal static Dictionary<string, Sector> AllSectors;
        internal static Dictionary<string, Mission> AllMissions;
        internal static Dictionary<string, Faction> AllFactions;
        internal static Dictionary<int, Filter.Build> AllBuilds;

        internal static (Dictionary<string, Item> data, int version) ParseItems(int oldVersion, string text) =>
            ParseText(oldVersion, text, cat: "Items", selector: (value, type, enabled) => new Item(value: value, type: type, enabled: enabled));

        //internal static (Dictionary<string, Sector> data, int version) ParseSectors(int oldVersion, string text) =>
        //    ParseText(oldVersion, text, cat: "Items", selector: (value, type, enabled) =>
        //        {
        //            var parts = value.Split('|');
        //            return new Sector(planet: parts[0], location: parts[1]);
        //        });



        internal static (Dictionary<string, Sector> data, int version) ParseSectors(int oldVersion, string text)
        {
            var model = JsonConvert.DeserializeObject<SectorsModel>(text);
            if (model.Version > oldVersion)
            {
                var parts = model.Items.ToDictionary(t => t.Key, t => new Sector(t.Value.Split('|')[0], t.Value.Split('|')[1]));
                return (parts, model.Version);
            }
            
            else
                return (null, model.Version);
        }

        internal static (Dictionary<string, Mission> data, int version) ParseMissions(int oldVersion, string text)
        {
            var model = JsonConvert.DeserializeObject<MissionsModel>(text);
            if (model.Version > oldVersion)
            {
                var parts = model.Missions.ToDictionary(t => t.Key, t => new Mission(t.Value));
                return (parts, model.Version);
            }

            else
                return (null, model.Version);
        }


        //internal static (Dictionary<string, Mission> data, int version) ParseMissions(int oldVersion, string text) =>
        //    ParseText(oldVersion, text, cat: "Missions", selector: (value, type, enabled) => new Mission(value));

        private static (Dictionary<string, T> data, int version) ParseText<T>(int oldVersion, string text, string cat, Func<string, string, bool, T> selector)
        {
            var json = JObject.Parse(text);
            var version = (int)json["Version"];
            if (version <= oldVersion)
                return (null, version);

            var result = json[cat]
                .SelectMany(s =>
                {
                    var type = (string)s["type"];
                    var enabled = ((int?)s["enable"] ?? 1) != 0;
                    return ((JObject)s).Properties()
                                            .Where(p => p.Name != "type" && p.Name != "enable")
                                            .Select(p => (key: p.Name, v: selector((string)p.Value, type, enabled)));
                })
                .ToDictionary(t => t.key, t => t.v);

            return (result, version);
        }

        internal static (Dictionary<string, Faction> data, int version) ParseFactions(int oldVersion, string text)
        {
            var model = JsonConvert.DeserializeObject<FactionsModel>(text);
            if (model.Version > oldVersion)
                return (model.Items, model.Version);
            else
                return (null, model.Version);
        }

        public class FactionsModel
        {
            public DateTime Date { get; set; }
            public int Version { get; set; }
            public Dictionary<string, Faction> Items { get; set; }
        }


        public class SectorsModel
        {
            public int Version { get; set; }
            public Dictionary<string, string> Items { get; set; }
        }

        public class MissionsModel
        {
            public int Version { get; set; }
            public Dictionary<string, string> Missions { get; set; }
        }

        internal static (Dictionary<int, Filter.Build> data, int version) ParseBuilds(int oldVersion, string text)
        {
            var model = JsonConvert.DeserializeObject<BuildsModel>(text);
            if (model.Version > oldVersion)
                return (model.Items, model.Version);
            else
                return (null, model.Version);
        }

        public class BuildsModel
        {
            public DateTime Date { get; set; }
            public int Version { get; set; }
            public Dictionary<int, Filter.Build> Items { get; set; }
        }
    }
}
