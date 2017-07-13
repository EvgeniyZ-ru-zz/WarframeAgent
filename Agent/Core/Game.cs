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
        }
    }
}