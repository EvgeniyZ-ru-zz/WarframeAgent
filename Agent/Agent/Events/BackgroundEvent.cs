using System;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.Events
{
    internal class BackgroundEvent
    {
        public delegate void MethodContainer();

        public static event MethodContainer Changed;

        private static void ChangeMethod()
        {
            if (Settings.Program.RandomBackground)
            {
                var rand = new Random();
                var randValue = rand.Next(1, 8);
                while (randValue == Settings.Program.BackgroundId)
                    randValue = rand.Next(1, 8);
                Settings.Program.BackgroundId = randValue;
                Settings.Program.Save();
            }
            Changed?.Invoke();
        }

        public static void Start()
        {
            var task = new Task(() =>
            {
                if (Settings.Program.RandomBackground)
                {
                    while (Settings.Program.RandomBackground)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(5));
                        ChangeMethod();
                    }
                } else
                {
                    ChangeMethod();
                }
                
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
