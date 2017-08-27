using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Core.Events
{
    public class FiltersEvent
    {
        public delegate void MethodContainer();

        private CancellationTokenSource _cts;
        private Task _mainTask;

        /// <summary>
        ///     Данные успешно обновлены.
        /// </summary>
        public event MethodContainer Updated;

        /// <summary>
        ///     Фильтр "Items" успешно обновлен.
        /// </summary>
        public event MethodContainer ItemsUpdated;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            await RunInitialPopulation(_cts.Token);
            _mainTask = RunFilterUpdateLoop(_cts.Token);
        }

        public async Task StopAsync()
        {
            _cts.Cancel();
            _cts = null;
            if (_mainTask != null)
                await _mainTask;
        }

        struct NameUriPair { public string name; public Uri url; }
        async Task<Dictionary<Model.Filter.Type, Uri>> FetchFilterAddresses()
        {
            var fetchUri = Settings.Program.Urls.Filter;
            Tools.Logging.Send(LogLevel.Debug, $"Фильтры: получаю адреса свежих фильтров из {fetchUri}");
            try
            {
                var fetchedJson = await Tools.Network.ReadTextAsync(fetchUri);
                Tools.Logging.Send(LogLevel.Debug, $"Фильтры: получил адреса свежих фильтров");
                var uris = await Task.Run(() => // parse in the background
                {
                    var values = JsonConvert.DeserializeObject<NameUriPair[]>(fetchedJson);
                    return values.ToDictionary(
                        p => (Model.Filter.Type)Enum.Parse(typeof(Model.Filter.Type), p.name, ignoreCase: true),
                        p => p.url);
                });

                var currentUris = Settings.Program.Filters.Content;
                // если есть отличия, запишем на диск
                if (!(uris.OrderBy(t => t.Key).SequenceEqual(currentUris.OrderBy(t => t.Key))))
                {
                    Tools.Logging.Send(LogLevel.Debug, "Фильтры: получены новые фильтры, сохраняю");
                    Settings.Program.Filters.Content = uris;
                    Settings.Program.Save();
                }

                return uris;
            }
            catch (Exception ex)
            {
                Tools.Logging.Send(LogLevel.Warn, $"Ошибка при загрузке и разборе списка адресов {fetchUri}.", ex);
                return Settings.Program.Filters.Content;
            }
        }

        static string GetFilterFilePath(Model.Filter.Type type) => $"Filters/{type}.json";
        static readonly IEnumerable<Model.Filter.Type> SupportedFilterTypes =
            new Model.Filter.Type[]
            {
                Model.Filter.Type.Items,
                Model.Filter.Type.Planets,
                Model.Filter.Type.Missions,
                Model.Filter.Type.Factions
            };

        Dictionary<Model.Filter.Type, int> versions = SupportedFilterTypes.ToDictionary(k => k, k => -1);

        async Task RunInitialPopulation(CancellationToken ct)
        {
            foreach (var type in SupportedFilterTypes)
            {
                string path = GetFilterFilePath(type);
                string filterText;
                try
                {
                    var expandedPath = Model.StorageModel.ExpandRelativeName(path);
                    filterText = await Tools.File.ReadAllTextAsync(expandedPath);
                }
                catch (Exception e)
                {
                    Tools.Logging.Send(LogLevel.Warn, $"Ошибка при чтении {path}.", e);
                    continue;
                }

                var currentVersion = versions[type];
                var newVersion = await TryUpdateFilter(currentVersion, type, filterText);
                versions[type] = newVersion;
            }
        }

        async Task RunFilterUpdateLoop(CancellationToken ct)
        {
            var uris = await FetchFilterAddresses();
            while (!ct.IsCancellationRequested)
            {
                foreach (var type in SupportedFilterTypes)
                {
                    var uri = uris[type];
                    var filterText = await Tools.Network.ReadTextAsync(uri);
                    var currentVersion = versions[type];
                    var newVersion = await TryUpdateFilter(currentVersion, type, filterText);
                    versions[type] = newVersion;
                }

                await Task.Delay(TimeSpan.FromMinutes(10), ct);
            }
        }

        async Task<int> TryUpdateFilter(int oldVersion, Model.Filter.Type type, string filterText)
        {
            try
            {
                switch (type)
                {
                case Model.Filter.Type.Items:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseItems(oldVersion, filterText));
                        if (data == null || version <= oldVersion)
                            return oldVersion;
                        Model.FiltersModel.AllItems = data;
                        return version;
                    }
                case Model.Filter.Type.Planets:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseSectors(oldVersion, filterText));
                        if (data == null || version <= oldVersion)
                            return oldVersion;
                        Model.FiltersModel.AllSectors = data;
                        return version;
                    }
                case Model.Filter.Type.Missions:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseMissions(oldVersion, filterText));
                        if (data == null || version <= oldVersion)
                            return oldVersion;
                        Model.FiltersModel.AllMissions = data;
                        return version;
                    }
                case Model.Filter.Type.Factions:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseFactions(oldVersion, filterText));
                        if (data == null || version <= oldVersion)
                            return oldVersion;
                        Model.FiltersModel.AllFactions = data;
                        return version;
                    }
                }
            }
            catch (Exception e)
            {
                Tools.Logging.Send(LogLevel.Warn, $"Ошибка при разборе фильтра типа {type}", e);
                return oldVersion;
            }

            throw new NotSupportedException($"Не имплементирована обработка типа фильтров {type}");
        }
    }
}