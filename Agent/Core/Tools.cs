using System;
using System.IO;
using System.Net;
using System.Text;

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
                catch (Exception)
                {
                    //TODO: Логирование
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
            public static void DownloadFile(string url, string patch)
            {
                using (var c = new WebClient())
                {
                    c.DownloadFile(url, patch);
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
    }
}
