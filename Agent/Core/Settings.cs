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
        public Core Core = new Core();
        public Verisons Verisons = new Verisons();
        public Directories Directories = new Directories();
        public Urls Urls = new Urls();
        public Themes Theme { get; set; }
        public bool RandomBackground { get; set; } = true;
        public int BackgroundId { get; set; } = 1;
    }

    public class Core
    {
        public bool UseGpu { get; set; } = true;
    }

    public class Verisons
    {
        public string Program { get; set; }
        public int Items { get; set; }
    }

    public class Directories
    {
        public string Data { get; set; } = "Data";
        public string Temp { get; set; } = "Data/Temp";
    }

    public class Urls
    {
        public Filters Filters = new Filters();
        public string Game { get; set; } = "http://content.warframe.com/dynamic/worldState.php";
        public string News { get; set; } = "https://www.warframe.com/ru/news/get_posts?page=1&category=pc";
    }

    public class Filters
    {
        public string Items { get; set; } =
            "https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Items.json";

        public string Missions { get; set; } =
            "https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Missions.json";

        public string Planets { get; set; } =
            "https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Planets.json";

        public string Race { get; set; } =
            "https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Race.json";

        public string Sorties { get; set; } =
            "https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Sorties.json";

        public string Void { get; set; } = "https://github.com/arrer/WarframeAgent/blob/master/Filters/Void.json";
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