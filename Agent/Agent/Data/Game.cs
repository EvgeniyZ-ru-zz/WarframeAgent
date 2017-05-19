using System;
using System.IO;
using System.Windows.Threading;
using Core.GameData;
using Newtonsoft.Json;
using Core;

namespace Agent.Data
{
    /// <summary>
    ///     Взаимодействие с игровыми данными.
    /// </summary>
    public class Game : VM
    {
        public Game()
        {
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, EventArgs e)
        {
            if (Data?.Alerts == null) return;
            for (var index = 0; index < (Data?.Alerts).Count; index++)
            {
                var item = (Data?.Alerts)[index];
                item.Status = null;
            }
        }

        private GameView Read(string fileName)
        {
            GameView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameView)serializer.Deserialize(file, typeof(GameView));
            }

            return data;
        }

        GameView _data;
        public GameView Data { get => _data; set => Set(ref _data, value); }

        public void Load(string filename = "temp")
        {
            if (filename == "temp")
                filename = $"{Settings.Program.Directories.Temp}/GameData.json";
            Data = Read(filename);

            foreach (var item in Data.Alerts)
            {
                item.Status = null;
            }
        }
    }
}
