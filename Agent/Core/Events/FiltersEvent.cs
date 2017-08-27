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
            try
            {
                await RunInitialPopulation(_cts.Token);
            }
            catch (OperationCanceledException ex) when (_cts.IsCancellationRequested)
            {
                // TODO: Log
                return;
            }
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
        async Task<Dictionary<Model.Filter.Type, Uri>> FetchFilterAddresses(CancellationToken ct)
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
                }, ct);
                Tools.Logging.Send(LogLevel.Debug, $"Фильтры: разбор адресов фильтров окончен");

                var currentUris = Settings.Program.Filters.Content;
                // если есть отличия, запишем на диск
                if (!(uris.OrderBy(t => t.Key).SequenceEqual(currentUris.OrderBy(t => t.Key))))
                {
                    Tools.Logging.Send(LogLevel.Debug, "Фильтры: получены новые адреса фильтры, сохраняю");
                    Settings.Program.Filters.Content = uris;
                    Settings.Program.Save();
                }
                else
                {
                    Tools.Logging.Send(LogLevel.Debug, $"Фильтры: изменений в адресах фильтров нет");
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
            Tools.Logging.Send(LogLevel.Debug, "Фильтры: начальная загрузка сохранённых фильтров");
            foreach (var type in SupportedFilterTypes)
            {
                ct.ThrowIfCancellationRequested();
                string path = GetFilterFilePath(type);
                Tools.Logging.Send(LogLevel.Debug, $"Фильтры: начальная загрузка фильтра {type} из файла {path}");
                string filterText;
                try
                {
                    var expandedPath = Model.StorageModel.ExpandRelativeName(path);
                    filterText = await Tools.File.ReadAllTextAsync(expandedPath);
                    Tools.Logging.Send(LogLevel.Debug, $"Фильтры: фильтр {type} прочитан из файла {path}");
                }
                catch (Exception e)
                {
                    Tools.Logging.Send(LogLevel.Warn, $"Ошибка при чтении {path}.", e);
                    continue;
                }

                ct.ThrowIfCancellationRequested();
                var currentVersion = versions[type];
                var newVersion = await TryUpdateFilter(currentVersion, type, filterText, ct);
                versions[type] = newVersion;
            }
            Tools.Logging.Send(LogLevel.Debug, "Фильтры: начальная загрузка сохранённых фильтров окончена");
        }

        async Task RunFilterUpdateLoop(CancellationToken ct)
        {
            Tools.Logging.Send(LogLevel.Debug, "Фильтры: запускаем цикл обновления");
            try
            {
                Tools.Logging.Send(LogLevel.Debug, "Фильтры: получаем адреса фильтров");
                var uris = await FetchFilterAddresses(ct);
                Tools.Logging.Send(LogLevel.Debug, "Фильтры: адреса фильтров получены");
                while (!ct.IsCancellationRequested)
                {
                    Tools.Logging.Send(LogLevel.Debug, "Фильтры: запрашиваем версию фильтров");
                    foreach (var type in SupportedFilterTypes)
                    {
                        var uri = uris[type];
                        Tools.Logging.Send(LogLevel.Debug, $"Фильтры: грузим фильтр {type} из {uri}");
                        var filterText = await Tools.Network.ReadTextAsync(uri, ct);
                        Tools.Logging.Send(LogLevel.Debug, $"Фильтры: фильтр {type} загружен");
                        var currentVersion = versions[type];
                        var newVersion = await TryUpdateFilter(currentVersion, type, filterText, ct);
                        if (newVersion > currentVersion)
                        {
                            string path = GetFilterFilePath(type);
                            var expandedPath = Model.StorageModel.ExpandRelativeName(path);
                            Tools.Logging.Send(LogLevel.Debug, $"Фильтры: фильтр {type} обновился, сохраняю в файл {path}");
                            await Tools.File.WriteAllTextAsync(expandedPath, filterText);
                        }
                        versions[type] = newVersion;
                    }
                    Tools.Logging.Send(LogLevel.Debug, "Фильтры: итерация обновления окончена");

                    await Task.Delay(TimeSpan.FromMinutes(10), ct);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                /* ничего не делаем, нас отменили */
            }
            Tools.Logging.Send(LogLevel.Debug, $"Обновление фильтров: цикл обновления завершён");
        }

        async Task<int> TryUpdateFilter(int oldVersion, Model.Filter.Type type, string filterText, CancellationToken ct)
        {
            try
            {
                Tools.Logging.Send(LogLevel.Debug, $"Фильтры: разбираю фильтр {type}");
                switch (type)
                {
                case Model.Filter.Type.Items:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseItems(oldVersion, filterText), ct);
                        ct.ThrowIfCancellationRequested();
                        if (data == null || version <= oldVersion)
                        {
                            Tools.Logging.Send(LogLevel.Debug, $"Фильтры: версия фильтра {type} не обновилась");
                            return oldVersion;
                        }
                        Tools.Logging.Send(LogLevel.Debug, $"Фильтры: заменяю фильтр {type} на версию {version}");
                        Model.FiltersModel.AllItems = data;
                        return version;
                    }
                case Model.Filter.Type.Planets:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseSectors(oldVersion, filterText), ct);
                        ct.ThrowIfCancellationRequested();
                        if (data == null || version <= oldVersion)
                        {
                            Tools.Logging.Send(LogLevel.Debug, $"Фильтры: версия фильтра {type} не обновилась");
                            return oldVersion;
                        }
                        Tools.Logging.Send(LogLevel.Debug, $"Фильтры: заменяю фильтр {type} на версию {version}");
                        Model.FiltersModel.AllSectors = data;
                        return version;
                    }
                case Model.Filter.Type.Missions:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseMissions(oldVersion, filterText), ct);
                        ct.ThrowIfCancellationRequested();
                        if (data == null || version <= oldVersion)
                        {
                            Tools.Logging.Send(LogLevel.Debug, $"Фильтры: версия фильтра {type} не обновилась");
                            return oldVersion;
                        }
                        Tools.Logging.Send(LogLevel.Debug, $"Фильтры: заменяю фильтр {type} на версию {version}");
                        Model.FiltersModel.AllMissions = data;
                        return version;
                    }
                case Model.Filter.Type.Factions:
                    {
                        (var data, var version) = await Task.Run(() => Model.FiltersModel.ParseFactions(oldVersion, filterText), ct);
                        ct.ThrowIfCancellationRequested();
                        if (data == null || version <= oldVersion)
                        {
                            Tools.Logging.Send(LogLevel.Debug, $"Фильтры: версия фильтра {type} не обновилась");
                            return oldVersion;
                        }
                        Tools.Logging.Send(LogLevel.Debug, $"Фильтры: заменяю фильтр {type} на версию {version}");
                        Model.FiltersModel.AllFactions = data;
                        return version;
                    }
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
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