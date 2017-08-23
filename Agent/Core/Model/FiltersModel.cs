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
        static T Expand<T>(string item, Dictionary<string, T> dict) where T : class
        {
            if (item == null)
                return null;
            if (dict == null)
                return null;
            bool isFilterFood = dict.TryGetValue(item, out var result);
            if (!isFilterFood)
                BadFilterReportModel.ReportBadFilter(item, null);
            return result;
        }

        public static Item ExpandItem(string item) => Expand(item, FiltersModel.AllItems);
        public static Sector ExpandSector(string item) => Expand(item, FiltersModel.AllSectors);
        public static Mission ExpandMission(string item) => Expand(item, FiltersModel.AllMissions);
        public static Faction ExpandFaction(string item) => Expand(item, FiltersModel.AllFactions);
    }

    class FiltersModel
    {
        public static Dictionary<string, Item> AllItems =
            ParseFile("Filters/Items.json", "Items", (value, type, enabled) => new Item(value: value, type: type, enabled: enabled));

        public static Dictionary<string, Sector> AllSectors =
            ParseFile("Filters/Planets.json", "Items", (value, type, enabled) =>
                {
                    var parts = value.Split('|');
                    return new Sector(planet: parts[0], location: parts[1]);
                });

        public static Dictionary<string, Mission> AllMissions =
            ParseFile("Filters/Missions.json", "Missions", (value, type, enabled) => new Mission(value));

        public static Dictionary<string, Faction> AllFactions =
            GetAllFactions();

        private static Dictionary<string, T> ParseFile<T>(string file, string cat, Func<string, string, bool, T> selector)
        {
            try
            {
                var absoluteFile = StorageModel.ExpandRelativeName(file);
                var strings = File.ReadAllText(absoluteFile, Encoding.UTF8);
                var json = JObject.Parse(strings);
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

                return result;
            }
            catch (Exception e)
            {
                Tools.Logging.Send(LogLevel.Warn, $"Ошибка при чтении {file}.", e);
                return new Dictionary<string, T>();
            }
        }

        public static Dictionary<string, Faction> GetAllFactions()
        {
            try
            {
                var absoluteFile = StorageModel.ExpandRelativeName("Filters/Factions.json");
                JsonSerializer s = JsonSerializer.CreateDefault();
                using (var text = File.OpenText(absoluteFile))
                using (var jreader = new JsonTextReader(text))
                {
                    var model = s.Deserialize<FactionsModel>(jreader);
                    return model.Items;
                }
            }
            catch (Exception e)
            {
                Tools.Logging.Send(LogLevel.Warn, "Ошибка чтения фракций", e);
                return new Dictionary<string, Faction>();
            }
        }

        public class FactionsModel
        {
            public DateTime Date { get; set; }
            public int Version { get; set; }
            public Dictionary<string, Faction> Items { get; set; }
        }
    }
}
