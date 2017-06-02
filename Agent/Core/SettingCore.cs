using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Core
{
    /// <summary>
    ///     Чтение/запись настроек из JSON файла
    /// </summary>
    /// <typeparam name="T">Класс</typeparam>
    public class SettingCore<T> where T : new()
    {
        private const string DefaultFile = "Settings.json";

        #region Load

        /// <summary>
        ///     Загружает настройки из JSON файла.
        /// </summary>
        /// <param name="fileName">Название JSON файла</param>
        /// <returns>Object</returns>
        public static T Load(string fileName = DefaultFile)
        {
            var t = new T();
            if (File.Exists(fileName))
                try
                {
                    t = JObject.Parse(File.ReadAllText(fileName)).ToObject<T>();
                }
                catch (Exception)
                {
                    File.Delete(fileName);
                }
            return t;
        }

        #endregion

        #region Save

        /// <summary>
        ///     Сохранение настроек в файл.
        /// </summary>
        /// <param name="fileName">Название JSON файла</param>
        public void Save(string fileName = DefaultFile)
        {
            File.WriteAllText(fileName, JObject.FromObject(this).ToString());
        }

        /// <summary>
        ///     Сохранение объекта в JSON файл
        /// </summary>
        /// <param name="pSettings">Объект для записи</param>
        /// <param name="fileName">Название JSON файла</param>
        public static void Save(T pSettings, string fileName = DefaultFile)
        {
            File.WriteAllText(fileName, JObject.FromObject(pSettings).ToString());
        }

        #endregion
    }
}