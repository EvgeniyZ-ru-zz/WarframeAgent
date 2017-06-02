using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Events
{
    internal class FiltersEvent
    {
        public delegate void MethodContainer();

        private CancellationTokenSource _cts;

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
            Task.Run(() => Control(_cts.Token));
        }

        public void Stop()
        {
            _cts.Cancel();
            _cts = null;
        }

        private async void Control(CancellationToken ct)
        {
            //while (!ct.IsCancellationRequested)
            //{

            //    if (Tools.Network.Ping(Settings.Program.Urls.Filters.Items))
            //    {
            //        //int ServerVersion = 

            //        //Tools.Network.DownloadFile(Settings.Program.Urls.Filters.Items,
            //        //    $"{Settings.Program.Directories.Data}/Filters/Items.json");

            //        //GetVersion();

            //    }


            //    await Task.Delay(TimeSpan.FromMinutes(10), ct);
            //}
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