using Core;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.Events
{
    public class GlobalEvents
    {
        /// <summary>
        /// Проверка доступности соединения с сервером игры.
        /// </summary>
        public static class Connection
        {
            public delegate void MethodContainer();
            public static event MethodContainer Disconnected;
            public static event MethodContainer Connected;

            public static void Start()
            {
                var task = new Task(() =>
                {

                    bool isConnected = false;
                    bool isFirst = true;
                    while (true)
                    {
                        if (Tools.Network.Ping(Settings.Program.Urls.Game))
                        {
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
                        }

                        isFirst = false;
                        Thread.Sleep(30000);
                    }
                });
                task.Start();
            }
        }

        /// <summary>
        /// Обновление игровых данных.
        /// </summary>
        public static class GameDataUpdate
        {
            public delegate void MethodContainer();
            public static event MethodContainer Updated;

            public static void Start()
            {
                bool isFirst = true;
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
                        }
                        else
                        {
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
