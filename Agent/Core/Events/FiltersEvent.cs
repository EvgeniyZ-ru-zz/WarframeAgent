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
    internal class FiltersEvent
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

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _mainTask = Run(_cts.Token);
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts = null;
        }

        async Task<Dictionary<Model.Filter.Type, Uri>> FetchFilterAddresses()
        {
            var fetchUri = Settings.Program.Urls.Filter;
            try
            {
                var fetchedJson = await Tools.Network.ReadTextAsync(fetchUri);
                var uris = await Task.Run(() => // parse in the background
                {
                    var values = JsonConvert.DeserializeObject<(string name, Uri url)[]>(fetchedJson);
                    return values.ToDictionary(
                        p => (Model.Filter.Type)Enum.Parse(typeof(Model.Filter.Type), p.name, ignoreCase: true),
                        p => p.url);
                });

                var currentUris = Settings.Program.Filters.Content;
                // если есть отличия, запишем на диск
                if (!(uris.OrderBy(t => t.Key).SequenceEqual(currentUris.OrderBy(t => t.Key))))
                {
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

        async Task Run(CancellationToken ct)
        {
            var uris = await FetchFilterAddresses();
            while (!ct.IsCancellationRequested)
            {

            //    if (Tools.Network.Ping(Settings.Program.Urls.Filters.Items))
            //    {
            //        //int ServerVersion = 

            //        //Tools.Network.DownloadFile(Settings.Program.Urls.Filters.Items,
            //        //    $"{Settings.Program.Directories.Data}/Filters/Items.json");

            //        //GetVersion();

            //    }


                await Task.Delay(TimeSpan.FromMinutes(10), ct);
            }
        }

        private int GetVersion(string path, string json = null)
        {
            try
            {
                int version;
                using (var reader = new StreamReader(path))
                {
                    var data = (JObject) JsonConvert.DeserializeObject(reader.ReadToEnd());
                    version = (int) data["Version"];
                }
                return version;
            }
            catch (Exception)
            {
                //TODO: LOG
                return 0;
            }
        }
    }
}