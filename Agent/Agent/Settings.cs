using Core;

namespace Agent
{
    #region ViewModel Setting

    /// <summary>
    ///     ViewModel для настроек.
    /// </summary>
    internal class MainSettings : SettingCore<MainSettings>
    {
        public string Test { get; set; } = "Test";
    }

    #endregion

    #region Settings class 

    /// <summary>
    ///     Класс настроек приложения
    /// </summary>
    internal static class Settings
    {
        /// <summary>
        ///     Настройки программы.
        /// </summary>
        public static MainSettings Program;

        /// <summary>
        ///     Выполняем загрузку всех необходимых настроек из файла/сервера.
        /// </summary>
        public static void Load()
        {
            Program = MainSettings.Load();
        }
    }

    #endregion
}
