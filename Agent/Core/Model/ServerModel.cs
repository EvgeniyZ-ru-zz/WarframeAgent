using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Model
{
    public class ServerModel
    {
        public ServerModel(Events.GlobalEvents.ServerEvents events)
        {
            this.events = events;
        }

        private CancellationTokenSource _cts;
        private object mutex = new object();
        private Events.GlobalEvents.ServerEvents events;

        /// <summary>
        ///     Запуск обновления данных.
        /// </summary>
        public void Start()
        {
            lock (mutex)
            {
                _cts = new CancellationTokenSource();
                Task.Run(() => Control(_cts.Token));
            }
        }

        public void Stop()
        {
            lock (mutex)
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        private async void Control(CancellationToken ct)
        {
            var isFirst = true;
            var isConnected = false;
            var tempDir = Settings.Program.Directories.Temp;

            while (!ct.IsCancellationRequested)
            {
                if (Tools.Network.Ping(Settings.Program.Urls.Game))
                {
                    if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                    if (Tools.Network.Ping(Settings.Program.Urls.News) && isFirst)
                        Tools.Network.DownloadFile(Settings.Program.Urls.News, $"{tempDir}/NewsData.json");

                    Tools.Network.DownloadFile(Settings.Program.Urls.Game, $"{tempDir}/GameData.json");
                    events.RaiseUpdate();
                    if (!isConnected)
                    {
                        isConnected = true;
                        events.ReportConnectStatus(true);
                    }

                    Debug.WriteLine("Data Updated!");
                }
                else
                {
                    if (isConnected || isFirst)
                    {
                        isConnected = false;
                        events.ReportConnectStatus(false);
                    }
                    //TODO: LOG
                }
                isFirst = false;
                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }
    }
}
