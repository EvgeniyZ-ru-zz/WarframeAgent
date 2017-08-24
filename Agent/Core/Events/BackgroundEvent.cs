using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Events
{
    public class BackgroundEvent
    {
        public delegate void MethodContainer();

        public static event MethodContainer Changed;

        private static void ChangeMethod()
        {
            if (Settings.Program.Configure.RandomBackground)
            {
                var rand = new Random();
                var randValue = rand.Next(1, 8);
                while (randValue == Settings.Program.Data.BackgroundId)
                    randValue = rand.Next(1, 8);
                Settings.Program.Data.BackgroundId = randValue;
                Settings.Program.Save();
            }
            Changed?.Invoke();
        }

        public static void Start()
        {
            var task = new Task(() =>
            {
                if (Settings.Program.Configure.RandomBackground)
                    while (Settings.Program.Configure.RandomBackground)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(5));
                        ChangeMethod();
                    }
                else
                    ChangeMethod();
            });
            task.Start();
        }

        public static void Restart()
        {
            var task = new Task(ChangeMethod);
            task.Start();
        }
    }
}