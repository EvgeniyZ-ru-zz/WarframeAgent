using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.GameData
{
    #region View

    public class GameJsonView
    {
        public int Version { get; set; }
        public string MobileVersion { get; set; }
        public string BuildLabel { get; set; }
        public int Time { get; set; }
        public int Date { get; set; }
        public List<Alert> Alerts { get; set; }
        public List<double> ProjectPct { get; set; }
        public string WorldSeed { get; set; }
    }

    #region Alert

    public class Alert
    {
        public Id _id { get; set; }
        public Activation Activation { get; set; }
        public Expiry Expiry { get; set; }
        public MissionInfo MissionInfo { get; set; }
    }

    public class Id
    {
        [JsonProperty("$oid")]
        public string oid { get; set; }
    }

    public class Date
    {
        [JsonProperty("$numberLong")]
        public string NumberLongStr { get; set; }

        public long NumberLong
        {
            get
            {
                return long.Parse(NumberLongStr);
            }

            set
            {
                NumberLongStr = value.ToString();
            }
        }
    }

    public class Activation
    {
        [JsonProperty("$date")]
        public Date date { get; set; }
    }

    public class Expiry
    {
        [JsonProperty("$date")]
        public Date date { get; set; }
    }

    public class MissionInfo
    {
        public string missionType { get; set; }
        public string faction { get; set; }
        public string location { get; set; }
        public string levelOverride { get; set; }
        public string enemySpec { get; set; }
        public int minEnemyLevel { get; set; }
        public int maxEnemyLevel { get; set; }
        public double difficulty { get; set; }
        public int seed { get; set; }
        public int maxWaveNum { get; set; }
        public MissionReward missionReward { get; set; }
        public string extraEnemySpec { get; set; }
    }

    public class MissionReward
    {
        public int credits { get; set; }
        public List<CountedItem> countedItems { get; set; }
    }

    public class CountedItem
    {
        public string ItemType { get; set; }
        public int ItemCount { get; set; }
    }

    #endregion

    #endregion
}
