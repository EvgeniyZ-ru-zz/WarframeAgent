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
    public static class Filters
    {
        public static (string value, string type) ExpandItem(string item) =>
            FiltersModel.AllItems.TryGetValue(item, out var result) ? result : (item, null);

        public static (string planet, string location) ExpandSector(string item) =>
            FiltersModel.AllSectors.TryGetValue(item, out var result) ? result : (null, item);

        public static string ExpandMission(string item) =>
            FiltersModel.AllMissions.TryGetValue(item, out var result) ? result : item;

        public static (string name, string color, string logo)? TryExpandFaction(string item) =>
            FiltersModel.AllFactions.TryGetValue(item, out var result) ? result : default((string name, string color, string logo));
    }

    public class FiltersModel
    {
        public static Dictionary<string, (string value, string type)> AllItems =
            ParseFile("Filters/Items.json", "Items", pair => pair);

        public static Dictionary<string, (string planet, string location)> AllSectors =
            ParseFile("Filters/Planets.json", "Items", pair =>
                {
                    var parts = pair.value.Split('|');
                    return (planet: parts[0], location: parts[1]);
                });

        public static Dictionary<string, string> AllMissions =
            ParseFile("Filters/Missions.json", "Missions", pair => pair.value);

        public static Dictionary<string, (string name, string color, string logo)> AllFactions =
            GetAllFactions();

        private static Dictionary<string, T> ParseFile<T>(string file, string cat, Func<(string value, string type), T> selector)
        {
            try
            {
                var absoluteFile = StorageModel.ExpandRelativeName(file);
                var strings = File.ReadAllText(absoluteFile, Encoding.UTF8);
                var json = JObject.Parse(strings);
                var result = json[cat]
                    //.Where(s => ((int?)s["enable"] ?? 1) == 1)
                    .SelectMany(s =>
                    {
                        var type = (string)s["type"];
                        return ((JObject)s).Properties()
                                               .Where(p => p.Name != "type" && p.Name != "enable")
                                               .Select(p => (key: p.Name, v: selector((value: (string)p.Value, type: type))));
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

        public static Dictionary<string, (string name, string color, string logo)> GetAllFactions()
        {
            try
            {
                var absoluteFile = StorageModel.ExpandRelativeName("Filters/Factions.json");
                JsonSerializer s = JsonSerializer.CreateDefault();
                using (var text = File.OpenText(absoluteFile))
                using (var jreader = new JsonTextReader(text))
                {
                    var model = s.Deserialize<FactionsModel>(jreader);
                    return model.Items.ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.Name, kvp.Value.Color, kvp.Value.Logo));
                }
            }
            catch (Exception e)
            {
                Tools.Logging.Send(LogLevel.Warn, "Ошибка чтения фракций", e);
                return new Dictionary<string, (string name, string color, string logo)>();
            }
        }

        public class FactionInfo
        {
            public string Name { get; set; }
            public string Color { get; set; }
            public string Logo { get; set; }
        }

        public class FactionsModel
        {
            public DateTime Date { get; set; }
            public int Version { get; set; }
            public Dictionary<string, FactionInfo> Items { get; set; }
        }
    }
}
