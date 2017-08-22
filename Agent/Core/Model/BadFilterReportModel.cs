using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Core.Model
{
    class BadFilterReportModel
    {
        readonly BufferBlock<(string filter, string type)> reportQueue = new BufferBlock<(string filter, string type)>();
        Task consumerTask;

        public BadFilterReportModel()
        {
            consumerTask = Consumer();
        }

        public Task ReportBadFilter(string filter, string type) => reportQueue.SendAsync((filter, type));

        async Task Consumer()
        {
            var q = reportQueue;
            while (await q.OutputAvailableAsync())
            {
                var (filter, type) = await q.ReceiveAsync();
                await Tools.Network.SendPut(filter, type, "1");
            }
        }

        public async Task DisposeAsync()
        {
            reportQueue.Complete();
            await consumerTask;
        }
    }
}
