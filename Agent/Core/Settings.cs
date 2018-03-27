using System;
using System.Collections.Generic;

namespace Core
{
    public enum Themes
    {
        Light,
        Dark
    }

    #region Model Setting

    /// <summary>
    ///     Model для настроек.
    /// </summary>
    public class MainSettings : SettingCore<MainSettings>
    {
        public Data Data = new Data();
        public Configure Configure = new Configure();
        public Directories Directories = new Directories();
        public Urls Urls = new Urls();
        public Filters Filters = new Filters();
        public UserNotifications UserNotifications = new UserNotifications();
    }

    public class Directories
    {
        public string Data { get; set; } = "Data";
        public string Temp { get; set; } = "Data/Temp";
    }

    public class Urls
    {
        public Uri Game { get; set; } = new Uri("http://content.warframe.com/dynamic/worldState.php");
        public Uri News { get; set; } = new Uri("https://www.warframe.com/ru/news/get_posts?page=1&category=pc");
        public Uri Filter { get; set; } = new Uri("http://evgeniy-z.ru/api/v2/Agent/GetFiltersUrl?lang=rus");
    }

    public class Data
    {
        public string Version { get; set; }
        public int BackgroundId { get; set; } = 1;
        public static readonly int MinBackgroundId = 1;
        public static readonly int MaxBackgroundId = 7;
    }

    public class Configure
    {
        public bool RandomBackground { get; set; } = true;
        public bool UseGpu { get; set; } = true;
        public Themes Theme { get; set; }
    }

    public class Filters
    {
        public Dictionary<Model.Filter.Type, Uri> Content { get; set; } =
            new Dictionary<Model.Filter.Type, Uri>()
            {
                [Model.Filter.Type.Planets]  = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Planets.json"),
                [Model.Filter.Type.Race]     = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Race.json"),
                [Model.Filter.Type.Missions] = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Missions.json"),
                [Model.Filter.Type.Items]    = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Items.json"),
                [Model.Filter.Type.Factions] = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Factions.json"),
                [Model.Filter.Type.Sorties]  = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Sorties.json"),
                [Model.Filter.Type.Void]     = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Void.json"),
                [Model.Filter.Type.Builds]   = new Uri("https://raw.githubusercontent.com/arrer/WarframeAgent/master/Filters/Builds.json")
            };
    }

    public class UserNotifications
    {
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Dictionary<string, HashSet<string>> ById { get; set; } = new Dictionary<string, HashSet<string>>();
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