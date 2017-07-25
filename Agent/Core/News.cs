using System.IO;
using System.Threading.Tasks;
using Core.Model;
using Core.ViewModel;
using Newtonsoft.Json;

namespace Core.ViewModel
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
        public async void Load(string fileName)
        {
            NewsModel data = await Task.Run(() =>
            {
                using (var file = File.OpenText(fileName))
                {
                    var serializer = new JsonSerializer();
                    return (NewsModel) serializer.Deserialize(file, typeof(NewsModel));
                }
            });

            Data = data;
        }
    }
}