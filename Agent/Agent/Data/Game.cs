using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
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
            var timer = new Timer { Interval = 1000 };
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Data?.Alerts == null) return;
            foreach (var item in Data?.Alerts)
            {
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
