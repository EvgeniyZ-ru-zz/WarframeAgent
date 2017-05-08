using System.Collections.Generic;
using Newtonsoft.Json;
using Agent.Data;
using System.Collections.ObjectModel;

namespace Core.GameData
{
    #region View

    public class GameView : VM
    {
        public int Version { get; set; }
        public string MobileVersion { get; set; }
        public string BuildLabel { get; set; }
        public int Time { get; set; }
        public int Date { get; set; }
        ObservableCollection<Alert> alerts;
        public ObservableCollection<Alert> Alerts
        {
            get => alerts;
            set => Set(ref alerts, value);
        }
        public List<double> ProjectPct { get; set; }
        public string WorldSeed { get; set; }
    }

    #region Alert

    public class Alert
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }
        public Activation Activation { get; set; }
        public Expiry Expiry { get; set; }
        public MissionInfo MissionInfo { get; set; }
    }

    public class Id
    {
        [JsonProperty("$oid")]
        public string Oid { get; set; }
    }

    public class Date
    {
        [JsonProperty("$numberLong")]
        public string NumberLongStr { get; set; }

        public long NumberLong
        {
            get => long.Parse(NumberLongStr);
            set => NumberLongStr = value.ToString();
        }
    }

    public class Activation
    {
        [JsonProperty("$date")]
        public Date Date { get; set; }
    }

    public class Expiry
    {
        [JsonProperty("$date")]
        public Date Date { get; set; }
    }

    public class MissionInfo
    {
        public string MissionType { get; set; }
        public string Faction { get; set; }
        public string Location { get; set; }
        public string LevelOverride { get; set; }
        public string EnemySpec { get; set; }
        public int MinEnemyLevel { get; set; }
        public int MaxEnemyLevel { get; set; }
        public double Difficulty { get; set; }
        public int Seed { get; set; }
        public int MaxWaveNum { get; set; }
        public MissionReward MissionReward { get; set; }
        public string ExtraEnemySpec { get; set; }
        public List<string> CustomAdvancedSpawners { get; set; }
        public bool? ArchwingRequired { get; set; }
        public bool? IsSharkwingMission { get; set; }
    }

    public class MissionReward
    {
        public int Credits { get; set; }
        public List<CountedItem> CountedItems { get; set; }
    }

    public class CountedItem
    {
        public string ItemType { get; set; }
        public int ItemCount { get; set; }
    }

    #endregion

    #endregion
}
