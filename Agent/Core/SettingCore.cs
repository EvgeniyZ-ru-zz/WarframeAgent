using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                        return t;
                    }
                }
                catch (Exception)
                {
                    File.Delete(expandedFilename);
                }
            }
            return new T() { expandedFilename = expandedFilename };
        }

        #endregion

        #region Save

        public void Save()
        {
            File.WriteAllText(expandedFilename, JObject.FromObject(this).ToString());
        }

        #endregion
    }
}