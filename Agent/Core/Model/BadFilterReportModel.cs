using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NLog;

namespace Core.Model
{
    public class BadFilterReportModel
    {
        static BadFilterReportModel Instance = new BadFilterReportModel();
        public static void Start() { }
        public static Task StopAsync() => Instance.DisposeAsync();
        public static void ReportBadFilter(string filter, Filter.Type type) => Instance.ReportBadFilterImpl(filter, type);

        readonly BufferBlock<(string filter, Filter.Type type)> reportQueue = new BufferBlock<(string filter, Filter.Type type)>();
        readonly Task consumerTask;
        readonly TaskCompletionSource<bool> addingTask = new TaskCompletionSource<bool>();

        BadFilterReportModel()
        {
            consumerTask = Consumer();
        }

        void ReportBadFilterImpl(string filter, Filter.Type type)
        {
            reportQueue.Post((filter, type));
            Tools.Logging.Send(LogLevel.Debug, $"Добавлен плохой фильтр в очередь: {filter}, тип: {type}");
        }

        async Task Consumer()
        {
            Tools.Logging.Send(LogLevel.Debug, "Стартует очередь отправки фильтров");

            var q = reportQueue;
            var addingReady = addingTask.Task;

            await Tools.Async.RedirectToThreadPool();

            while (!addingReady.IsCompleted)
            {
                Tools.Logging.Send(LogLevel.Debug, "Очередь фильтров ждёт");
                // пауза 5 min
                await Task.WhenAny(addingReady, Task.Delay(TimeSpan.FromMinutes(5)));
                // выгрести всё, что в очереди
                Tools.Logging.Send(LogLevel.Debug, "Очередь фильтров проснулась, проверка очереди");
                var unsent = new List<(string filter, Filter.Type type)>();
                while (q.TryReceive(out var item))
                {
                    // TODO: проверить в файле
                    Tools.Logging.Send(LogLevel.Debug, $"Очередь фильтров, отправка фильтра {item.filter}, тип {item.type}");
                    var sendSuccessful = await Tools.Network.SendPut(item.filter, item.type.ToString(), Settings.Program.Data.Version);
                    if (sendSuccessful)
                    {
                        Tools.Logging.Send(LogLevel.Debug, "Очередь фильтров, отправка успешна");
                        // TODO: записать в файл
                    }
                    else
                    {
                        Tools.Logging.Send(LogLevel.Debug, "Очередь фильтров, отправка неудачна, фильтр будет отправлен повторно");
                        unsent.Add(item);
                        break; // while (q.TryReceive)
                    }
                }
                if (addingReady.IsCompleted)
                    break; // while (!finished.IsCompleted)
                foreach (var item in unsent)
                    q.Post(item); // вернёт false если очередь не принимает больше ничего
            }

            Tools.Logging.Send(LogLevel.Debug, "Завершилась очередь отправки фильтров");
        }

        async Task DisposeAsync()
        {
            Tools.Logging.Send(LogLevel.Debug, "Закрываю очередь отправки фильтров");
            addingTask.SetResult(true);
            reportQueue.Complete();
            Tools.Logging.Send(LogLevel.Debug, "Ожидаю остановки очереди");
            await consumerTask;
            Tools.Logging.Send(LogLevel.Debug, "Очередь прощается с вами");
        }
    }
}
