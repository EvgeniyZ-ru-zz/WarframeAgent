using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Core
{
    /// <summary>
    ///     Чтение/запись настроек из JSON файла
    /// </summary>
    /// <typeparam name="T">Класс</typeparam>
    public class SettingCore<T> where T : SettingCore<T>, new()
    {
        private const string DefaultFile = "Settings.json";

        [JsonIgnore]
        private string expandedFilename;

        #region Load

        public static T Load(string filename = DefaultFile)
        {
            var expandedFilename = Model.StorageModel.ExpandRelativeName(filename);
            if (File.Exists(expandedFilename))
            {
                try
                {
                    using (var text = File.OpenText(expandedFilename))
                    using (var jtext = new JsonTextReader(text))
                    {
                        var t = JObject.Load(jtext).ToObject<T>();
                        t.expandedFilename = expandedFilename;
                        Tools.Logging.Send(LogLevel.Info, $"Настройки: файл с настройками прочитан успешно");
                        return t;
                    }
                }
                catch (Exception ex)
                {
                    Tools.Logging.Send(LogLevel.Error, $"Настройки: ошибка при чтении файла настроек, удаляю файл", ex);
                }

                // если мы здесь, чтение настроек не удалось, файл повреждён?
                try
                {
                    File.Delete(expandedFilename);
                }
                catch (Exception ex)
                {
                    Tools.Logging.Send(LogLevel.Error, $"Настройки: ошибка при удалении файла настроек", ex);
                }
            }
            else
            {
                Tools.Logging.Send(LogLevel.Info, $"Настройки: файл с настройками не найден");
            }

            Tools.Logging.Send(LogLevel.Info, $"Настройки: созданы настройки по умолчанию");
            return new T() { expandedFilename = expandedFilename };
        }

        #endregion

        #region Save

        public void Save()
        {
            try
            {
                Tools.Logging.Send(LogLevel.Trace, $"Настройки: сохраняю в файл");
                File.WriteAllText(expandedFilename, JObject.FromObject(this).ToString());
                Tools.Logging.Send(LogLevel.Info, $"Настройки: сохранение произведено успешно");
            }
            catch (UnauthorizedAccessException e)
            {
                Tools.Logging.Send(LogLevel.Error, $"Настройки: недостаточно прав для сохранения в файл {expandedFilename}", e);
            }
            catch (IOException e)
            {
                Tools.Logging.Send(LogLevel.Error, $"Настройки: не могу сохранить в файл {expandedFilename}", e);
            }
        }

        #endregion
    }
}