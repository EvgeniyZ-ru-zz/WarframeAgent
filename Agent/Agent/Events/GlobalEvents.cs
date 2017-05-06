using Core;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.Events
{
    public class GlobalEvents
    {
        /// <summary>
        /// Обновление игровых данных.
        /// </summary>
        public static class GameDataEvent
        {
            public delegate void MethodContainer();
            /// <summary>
            /// Данные успешно обновлены.
            /// </summary>
            public static event MethodContainer Updated;
            
            /// <summary>
            /// Невозможно подключится к серверу.
            /// </summary>
            public static event MethodContainer Disconnected;

            /// <summary>
            /// Успешно подключились к серверу.
            /// </summary>
            public static event MethodContainer Connected;

            /// <summary>
            /// Запуск обновления данных.
            /// </summary>
            public static void Start()
            {
                bool isFirst = true;
                bool isConnected = false;
                var temp_dir = Settings.Program.Directories.Temp;

                var task = new Task(() =>
                {
                    while (true)
                    {
                        if (Tools.Network.Ping(Settings.Program.Urls.Game))
                        {
                            if (!Directory.Exists(temp_dir)) Directory.CreateDirectory(temp_dir);
                            if (Tools.Network.Ping(Settings.Program.Urls.News) && isFirst)
                            {
                                Tools.Network.DownloadFile(Settings.Program.Urls.News, $"{temp_dir}/NewsData.json");
                            }

                            Tools.Network.DownloadFile(Settings.Program.Urls.Game, $"{temp_dir}/GameData.json");
                            Updated?.Invoke();
                            if (!isConnected)
                            {
                                isConnected = true;
                                Connected?.Invoke();
                            }

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
                        Thread.Sleep(60000);
                    }
                });
                task.Start();
            }
        }
    }
}
