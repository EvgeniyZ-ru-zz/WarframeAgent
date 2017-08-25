using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Events
{
    public static class BackgroundEvent
    {
        public static event Action<int> Changed;
        static Task settingsWatchTask;
        static CancellationTokenSource watchCts = new CancellationTokenSource(), delayCts;
        static Random rand = new Random();
        static readonly TimeSpan changeDelay = TimeSpan.FromSeconds(10);

        static void UpdateBackground()
        {
            int randValue = rand.Next(Data.MinBackgroundId, Data.MaxBackgroundId); // [min..max-1]
            if (randValue == Settings.Program.Data.BackgroundId)
                randValue++;
            Settings.Program.Data.BackgroundId = randValue;
            Settings.Program.Save();
            Changed?.Invoke(randValue);
        }

        public static void Start()
        {
            settingsWatchTask = WatchSettings(watchCts.Token);
        }

        public static async Task StopAsync()
        {
            watchCts.Cancel();
            await settingsWatchTask;
        }

        static async Task WatchSettings(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var forceUpdate = false;
                using (delayCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                {
                    try
                    {
                        await Task.Delay(changeDelay, delayCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        forceUpdate = true;
                    }
                }
                delayCts = null;
                if (ct.IsCancellationRequested)
                    break;
                if (Settings.Program.Configure.RandomBackground || forceUpdate)
                    UpdateBackground();
            }
        }

        public static void Restart()
        {
            delayCts?.Cancel();
        }
    }
}