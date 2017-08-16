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
            (FiltersModel.AllItems != null && FiltersModel.AllItems.TryGetValue(item, out var result)) ? result : (item, null);

        public static (string planet, string location) ExpandSector(string item) =>
            (FiltersModel.AllSectors != null && FiltersModel.AllSectors.TryGetValue(item, out var result)) ? result : (null, item);

        public static string ExpandMission(string item) =>
            (FiltersModel.AllMissions != null && FiltersModel.AllMissions.TryGetValue(item, out var result)) ? result : item;
    }

    class FiltersModel
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
                return null;
            }
        }

        public static class Factions
        {
            // TODO: закешировать это! не читать каждый раз
            public static Dictionary<string, FactionInfo> GetAll()
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
                    return new Dictionary<string, FactionInfo>();
                }
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
