using System;
using System.IO;
using System.Windows.Threading;
using Core.Model;
using Core.ViewModel;
using Newtonsoft.Json;

namespace Core
{
    public class Game : VM
    {
        private GameModel _data;

        public Game()
        {
            var reloadTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            reloadTimer.Tick += reloadTimer_Elapsed;
            reloadTimer.Start();
        }

        /// <summary>
        ///     Основные игровые данные.
        /// </summary>
        public GameModel Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        /// <summary>
        ///     Загружаем JSON файл с игровыми данными.
        /// </summary>
        /// <param name="fileName">Путь до JSON файла</param>
        public void Load(string fileName)
        {
            GameModel data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameModel) serializer.Deserialize(file, typeof(GameModel));
            }

            Data = data;

            if (Data?.Alerts == null) return;
            for (var index = 0; index < (Data?.Alerts).Count; index++)
            {
                var item = (Data?.Alerts)[index];
                item.Status = null;
            }
        }


        private void reloadTimer_Elapsed(object sender, EventArgs e)
        {
            if (Data?.Alerts == null) return;
            for (var index = 0; index < (Data?.Alerts).Count; index++)
            {
                var item = (Data?.Alerts)[index];
                item.Status = null;
            }
        }
    }
}