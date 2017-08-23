using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Core.Model
{
    public class BadFilterReportModel
    {
        static BadFilterReportModel Instance = new BadFilterReportModel();
        static public void Start() { }
        static public Task StopAsync() => Instance.DisposeAsync();
        static public void ReportBadFilter(string filter, Filter.Type type) => Instance.ReportBadFilterImpl(filter, type);

        readonly BufferBlock<(string filter, Filter.Type type)> reportQueue = new BufferBlock<(string filter, Filter.Type type)>();
        Task consumerTask;

        BadFilterReportModel()
        {
            consumerTask = Consumer();
        }

        void ReportBadFilterImpl(string filter, Filter.Type type) => reportQueue.Post((filter, type));

        async Task Consumer()
        {
            var q = reportQueue;
            var finished = q.Completion;

            await Tools.Async.RedirectToThreadPool();

            while (!finished.IsCompleted)
            {
                // пауза 5 min
                await Task.WhenAny(finished, Task.Delay(TimeSpan.FromMinutes(5)));
                // выгрести всё, что в очереди
                var unsent = new List<(string filter, Filter.Type type)>();
                while (q.TryReceive(out var item))
                {
                    // TODO: проверить в файле
                    var sendSuccessful = await Tools.Network.SendPut(item.filter, item.type.ToString(), "1"); // TODO: correct version
                    if (sendSuccessful)
                    {
                        // TODO: записать в файл
                    }
                    else
                    {
                        unsent.Add(item);
                    }
                }
                if (finished.IsCompleted)
                    break;
                foreach (var item in unsent)
                    q.Post(item); // вернёт false если очередь не принимает больше ничего
            }
        }

        async Task DisposeAsync()
        {
            reportQueue.Complete();
            await consumerTask;
        }
    }
}
