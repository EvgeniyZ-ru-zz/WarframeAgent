using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Events
{
    public class GlobalEvents
    {
        /// <summary>
        ///     Обновление игровых данных.
        /// </summary>
        public class GameDataEvent : EventArgs
        {
            public delegate void MethodContainer();

            private CancellationTokenSource _cts;

            /// <summary>
            ///     Данные успешно обновлены.
            /// </summary>
            public event MethodContainer Updated;

            /// <summary>
            ///     Невозможно подключится к серверу.
            /// </summary>
            public event MethodContainer Disconnected;

            /// <summary>
            ///     Успешно подключились к серверу.
            /// </summary>
            public event MethodContainer Connected;

            /// <summary>
            ///     Запуск обновления данных.
            /// </summary>
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
                        Updated?.Invoke();
                        if (!isConnected)
                        {
                            isConnected = true;
                            Connected?.Invoke();
                        }

                        Debug.WriteLine("Data Updated!");
                    }
                    else
                    {
                        if (isConnected || isFirst)
                        {
                            isConnected = false;
                            Disconnected?.Invoke();
                        }
                        //TODO: LOG
                    }
                    isFirst = false;
                    await Task.Delay(TimeSpan.FromMinutes(1), ct);
                }
            }
        }
    }
}