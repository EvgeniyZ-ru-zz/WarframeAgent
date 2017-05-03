using System.IO;
using Core.GameData;
using Newtonsoft.Json;

namespace Agent.Data
{
    /// <summary>
    ///     Взаимодействие с игровыми данными.
    /// </summary>
    internal class Game
    {
        private static GameJsonView Read(string fileName)
        {
            GameJsonView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameJsonView) serializer.Deserialize(file, typeof(GameJsonView));
            }

            return data;
        }

        /// <summary>
        ///     Основные игровые данные.
        /// </summary>
        public static GameJsonView Data;

        /// <summary>
        ///     Загружаем JSON файл с игровыми данными.
        /// </summary>
        /// <param name="filename">Путь до JSON файла</param>
        public static void Load(string filename)
        {
            Data = Read(filename);
        }
    }
}
