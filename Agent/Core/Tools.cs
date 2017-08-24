using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Core
{
    /// <summary>
    ///     Класс вспомогательных утилит.
    /// </summary>
    public class Tools
    {
        /// <summary>
        ///     Взаимодействия с интернетом.
        /// </summary>
        public class Network
        {
            #region Ping

            /// <summary>
            ///     Проверить адрес на доступность
            /// </summary>
            /// <param name="adress">Адрес</param>
            /// <param name="timeout">Задержка</param>
            /// <returns>true/false</returns>
            public static bool Ping(string adress, int timeout = 100000) //TODO: Нужна ли? Двойной запрос до сервера.
            {
                var statusCode = 0;
                if (adress == null) return false;
                try
                {
                    var request = (HttpWebRequest) WebRequest.Create(adress);
                    request.AllowAutoRedirect = true;
                    request.Timeout = timeout;
                    request.Method = WebRequestMethods.Http.Get;
                    request.Accept = @"*/*";
                    var response = (HttpWebResponse) request.GetResponse();
                    statusCode = (int) response.StatusCode;
                    response.Close();
                }
                catch (Exception e)
                {
                    Logging.Send(LogLevel.Warn, "Ping Error", e);
                }

                return statusCode == 200;
            }

            #endregion

            #region DownloadFile

            /// <summary>
            ///     Загружает файл в локальную директорию
            /// </summary>
            /// <param name="url">Адрес файла</param>
            /// <param name="patch">Куда сохранять</param>
            public static void DownloadFile(string url, string relativePath)
            {
                using (var c = new WebClient())
                {
                    c.DownloadFile(url, Model.StorageModel.ExpandRelativeName(relativePath));
                }
            }

            #endregion

            #region ReadText

            public static string ReadText(string url)
            {
                var wc = new WebClient {Encoding = Encoding.UTF8};
                return wc.DownloadString(url);
            }

            #endregion

            #region Send Bad Filter

            /// <summary>
            /// Отправляет Put запрос на указанный адрес
            /// </summary>
            /// <param name="data">Объект для сериализации в JSON</param>
            /// <param name="url">Адрес для отправки</param>
            public static Task<bool> SendPut(object data, string url = "https://evgeniy-z.ru/api/v2/agent/filters")
            {
                return PutRequest(data, url);
            }

            /// <summary>
            /// Отправляет Put запрос на указанный адрес
            /// </summary>
            /// <param name="name">Переменная для отрпавки</param>
            /// <param name="type">Тип (items, missions), соответсвует имени файла самого фильтра</param>
            /// <param name="version">Версия приложения</param>
            /// <param name="url">Адрес для отправки</param>
            public static Task<bool> SendPut(string name, string type, string version, string url = "https://evgeniy-z.ru/api/v2/agent/filters")
            {
                var data = new { Name = name, Type = type, Version = version };
                return PutRequest(data, url);
            }

            private static async Task<bool> PutRequest(object data, string url)
            {
                string serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                Logging.Send(LogLevel.Debug, $"Отправка на сервер объекта {serializedObject}");
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = "PUT";
                request.AllowWriteStreamBuffering = false;
                request.ContentType = "application/json";
                request.Accept = "Accept=application/json";
                request.SendChunked = false;
                request.ContentLength = serializedObject.Length;
                // request.Timeout doesn't work for asynchronous requests
                try
                {
                    var timeoutTask = Task.Delay(5000);
                    var putTask = PutWorker(request, serializedObject);

                    async Task<HttpStatusCode> PutWorker(HttpWebRequest req, string s)
                    {
                        using (var writer = new StreamWriter(await req.GetRequestStreamAsync()))
                        {
                            await writer.WriteAsync(s);
                            Logging.Send(LogLevel.Debug, "Отправка на сервер: запрос послан");
                        }
                        var response = (HttpWebResponse)(await req.GetResponseAsync());
                        return response.StatusCode;
                    }

                    var firstToFinish = await Task.WhenAny(timeoutTask, putTask);
                    if (firstToFinish == timeoutTask)
                    {
                        request.Abort();
                    }
                    else
                    {
                        var statusCode = await putTask;
                        switch (statusCode)
                        {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.Conflict:
                            Logging.Send(LogLevel.Debug, "Отправка на сервер: получено подтверждение");
                            return true;
                        }

                        Logging.Send(LogLevel.Warn, $"Отправка на сервер: получен неожиданный код ответа {statusCode}");
                    }
                }
                catch (WebException e) when (e.Response is HttpWebResponse hwr && hwr.StatusCode == HttpStatusCode.Conflict)
                {
                    Logging.Send(LogLevel.Debug, "Отправка на сервер: получено подтверждение");
                    return true;
                }
                catch (Exception e)
                {
                    Logging.Send(LogLevel.Warn, "Отправка на сервер: произошло исключение", e);
                }
                return false;
            }

            #endregion
        }

        /// <summary>
        ///     Работа со временем.
        /// </summary>
        public class Time
        {
            #region ToDateTime

            /// <summary>
            ///     Переводит UnixTime(ms) в DateTime
            /// </summary>
            /// <param name="timestamp">Unix время в формате long</param>
            /// <returns>DateTime</returns>
            public static DateTime ToDateTime(long timestamp)
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddMilliseconds(timestamp).ToLocalTime();
                return Convert.ToDateTime(dateTime);
            }

            #endregion

            #region ToUnixTime

            /// <summary>
            ///     Переводит DateTime в Unix (long)
            /// </summary>
            /// <param name="date">Время для перевода</param>
            /// <returns>Long</returns>
            public static long ToUnixTime(DateTime date)
            {
                var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var diff = date.ToUniversalTime() - origin;
                return Convert.ToInt64(diff.TotalMilliseconds);
            }

            #endregion
        }

        public class Logging
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
            public static void Send(LogLevel level, string message, Exception exception = null, object param = null)
            {
                string errorMsg = null;
                if (exception != null)
                    errorMsg = $"\n{exception}";

                Debug.WriteLine($"{message}{errorMsg}");
                Logger.Log(level, exception, message, param);
            }
        }

        /// <summary>
        ///   Асинхронная работа с потоками
        /// </summary>
        public class Async
        {
            public static ThreadPoolRedirector RedirectToThreadPool() =>
                default(ThreadPoolRedirector);

            public struct ThreadPoolRedirector : INotifyCompletion
            {
                public ThreadPoolRedirector GetAwaiter() => this;
                public bool IsCompleted => Thread.CurrentThread.IsThreadPoolThread;
                public void OnCompleted(Action continuation) => ThreadPool.QueueUserWorkItem(o => continuation());
                public void GetResult() { }
            }
        }

    }
}
