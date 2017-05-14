using System.IO;
using Core;
using Core.GameData;
using Newtonsoft.Json;

namespace Agent.Data
{
    /// <summary>
    ///     Взаимодействие с данными новостей.
    /// </summary>
    public class News : VM
    {
        private NewsView Read(string fileName)
        {
            NewsView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (NewsView) serializer.Deserialize(file, typeof(NewsView));
            }

            return data;
        }

        NewsView _data;
        /// <summary>
        ///     Основные данные новостей.
        /// </summary>
        public NewsView Data { get => _data; set => Set(ref _data, value); }

        /// <summary>
        ///     Загружаем JSON файл с данными новостей.
        /// </summary>
        /// <param name="filename">Путь до JSON файла</param>
        public void Load(string filename = "temp")
        {
            if (filename == "temp")
                filename = $"{Settings.Program.Directories.Temp}/NewsData.json";
            Data = Read(filename);
        }
    }
}
