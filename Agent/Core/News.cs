using System.IO;
using Core.Model;
using Core.ViewModel;
using Newtonsoft.Json;

namespace Core
{
    /// <summary>
    ///     Взаимодействие с данными новостей.
    /// </summary>
    public class News : VM
    {
        private NewsModel _data;

        /// <summary>
        ///     Основные данные новостей.
        /// </summary>
        public NewsModel Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        /// <summary>
        ///     Загружаем JSON файл с данными новостей.
        /// </summary>
        /// <param name="fileName">Путь до JSON файла</param>
        public void Load(string fileName)
        {
            NewsModel data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (NewsModel) serializer.Deserialize(file, typeof(NewsModel));
            }

            Data = data;
        }
    }
}