using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Agent
{
    /// <summary>
    ///   Асинхронная работа с потоками
    /// </summary>
    public class AsyncHelpers
    {
        public static DispatcherRedirector RedirectToMainThread() =>
            new DispatcherRedirector(Application.Current.Dispatcher);

        // http://blogs.msdn.com/b/pfxteam/archive/2011/01/13/10115642.aspx
        public struct DispatcherRedirector : System.Runtime.CompilerServices.INotifyCompletion
        {
            public DispatcherRedirector(Dispatcher dispatcher) =>
                this.dispatcher = dispatcher;

            // combined awaiter and awaitable
            public DispatcherRedirector GetAwaiter() => this;

            // true means execute continuation inline
            public bool IsCompleted => dispatcher.CheckAccess();
            public void OnCompleted(Action continuation) => dispatcher.BeginInvoke(continuation);
            public void GetResult() { }

            Dispatcher dispatcher;
        }
    }
}
