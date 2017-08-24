namespace Core
{
    public enum Themes
    {
        Light,
        Dark
    }

    #region ViewModel Setting

    /// <summary>
    ///     ViewModel для настроек.
    /// </summary>
    public class MainSettings : SettingCore<MainSettings>
    {
        public Data Data = new Data();
        public Configure Configure = new Configure();
        public Directories Directories = new Directories();
        public Urls Urls = new Urls();
    }

    public class Directories
    {
        public string Data { get; set; } = "Data";
        public string Temp { get; set; } = "Data/Temp";
    }

    public class Urls
    {
        public string Game { get; set; } = "http://content.warframe.com/dynamic/worldState.php";
        public string News { get; set; } = "https://www.warframe.com/ru/news/get_posts?page=1&category=pc";
    }

    public class Data
    {
        public string Version { get; set; }
        public int BackgroundId { get; set; } = 1;
    }

    public class Configure
    {
        public bool RandomBackground { get; set; } = true;
        public bool UseGpu { get; set; } = true;
        public Themes Theme { get; set; }
    }

    #endregion

    #region Settings class 

    /// <summary>
    ///     Класс настроек приложения
    /// </summary>
    public static class Settings
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