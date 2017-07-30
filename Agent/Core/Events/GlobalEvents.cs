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
        public class ServerEvents
        {
            public delegate void MethodContainer();

            private object mutex = new object();

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
            ///   Это свойство показывает, доступен ли сайт Settings.Program.Urls.Game
            ///   (null означает, что ещё не известно)
            /// </summary>
            private bool? isGameConnected;
            public bool? IsGameConnected
            {
                get { lock (mutex) return isGameConnected; }
                private set { lock (mutex) isGameConnected = value; }
            }

            internal void RaiseUpdate() => Updated?.Invoke();

            internal void ReportConnectStatus(bool isConnected)
            {
                IsGameConnected = isConnected;
                if (isConnected)
                    Connected?.Invoke();
                else
                    Disconnected?.Invoke();
            }
        }
    }
}