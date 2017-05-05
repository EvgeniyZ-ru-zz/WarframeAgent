using System.IO;
using Core.GameData;
using Newtonsoft.Json;

namespace Agent.Data
{
    /// <summary>
    ///     Взаимодействие с игровыми данными.
    /// </summary>
    internal class News
    {
        private static NewsView Read(string fileName)
        {
            NewsView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (NewsView) serializer.Deserialize(file, typeof(NewsView));
            }

            return data;
        }

        /// <summary>
        ///     Основные игровые данные.
        /// </summary>
        public static NewsView Data;

        /// <summary>
        ///     Загружаем JSON файл с игровыми данными.
        /// </summary>
        /// <param name="filename">Путь до JSON файла</param>
        public static void Load(string filename = "temp")
        {
            if (filename == "temp") filename = $"{Settings.Program.Directories.Temp}/NewsData.json";
            Data = Read(filename);
        }
    }
}
