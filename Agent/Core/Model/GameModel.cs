using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Core.Converters;
using Core.ViewModel;
using Newtonsoft.Json;

namespace Core.Model
{
    #region Main

    public class GameModel : VM
    {
        private GameModel _data;

        public GameModel Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        public int Version { get; set; }
        public string MobileVersion { get; set; }
        public string BuildLabel { get; set; }
        public int Time { get; set; }
        public int Date { get; set; }
        private ObservableCollection<Alert> _alerts;

        public ObservableCollection<Alert> Alerts
        {
            get => _alerts;
            set => Set(ref _alerts, value);
        }

        private ObservableCollection<Invasion> _invasions;

        public ObservableCollection<Invasion> Invasions
        {
            get
            {
                if (_invasions == null) return _invasions;
                _invasions = new ObservableCollection<Invasion>(_invasions.Where(p => !p.Completed));
                return _invasions;
            }
            set => Set(ref _invasions, value);
        }

        private ObservableCollection<ProjectsModel> _projectPct;

        [JsonConverter(typeof(ProjectConverter))]
        public ObservableCollection<ProjectsModel> ProjectPct
        {
            get => _projectPct;
            set => Set(ref _projectPct, value);
        }

        public string WorldSeed { get; set; }
    }

    #endregion

    #region Alert

    public class Alert : VM
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        public Activation Activation { get; set; }
        public Expiry Expiry { get; set; }
        public MissionInfo MissionInfo { get; set; }

        private string _status;

        public string Status
        {
            get => _status;
            set
            {
                var start = Tools.Time.ToDateTime(Activation.Date.NumberLong);
                var end = Tools.Time.ToDateTime(Expiry.Date.NumberLong);
                if (start >= DateTime.Now)
                {
                    value = (start - DateTime.Now).ToString(@"mm\:ss");
                }
                else
                {
                    if (DateTime.Now <= end)
                        if ((end - DateTime.Now.TimeOfDay).Hour == 0)
                            value = (end - DateTime.Now).ToString(@"mm\:ss");
                        else
                            value = (end - DateTime.Now).ToString(@"hh\:mm\:ss");
                    else
                        value = "Закончилось";
                }
                Set(ref _status, value);
            }
        }
    }

    public class MissionInfo
    {
        public string MissionType { get; set; }
        public string Faction { get; set; }
        public string Planet { get; set; }
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

        public string Reward
        {
            get
            {
                if (MissionReward.CountedItems != null)
                    return $"{MissionReward.CountedItems[0].ItemType} [{MissionReward.CountedItems[0].ItemCount}]";
                if (MissionReward.Items != null) return MissionReward.Items[0];
                return "Нет награды.";
            }
        }

        public Visibility ArchvingVisibility
        {
            get
            {
                if (ArchwingRequired == null || !ArchwingRequired.Value)
                    return Visibility.Collapsed;

                if (IsSharkwingMission != null && IsSharkwingMission.Value)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility SharkwingVisibility => IsSharkwingMission != null && IsSharkwingMission.Value
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility RewardVisibility => MissionReward.CountedItems != null || MissionReward.Items != null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public class MissionReward
    {
        public int Credits { get; set; }
        public List<CountedItem> CountedItems { get; set; }
        public List<string> Items { get; set; }
    }

    #endregion

    #region Invasions

    public class Invasion
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        public string Faction { get; set; }
        public string Node { get; set; }
        public double Count { get; set; }
        public double Goal { get; set; }

        public double Percent => DefenderMissionInfo.Faction == "FC_INFESTATION"
            ? (Goal + Count) / Goal * 100
            : (Goal + Count) / (Goal * 2) * 100;

        public string LocTag { get; set; }
        public bool Completed { get; set; }
        public object AttackerReward { get; set; }
        public InvasionMissionInfo AttackerMissionInfo { get; set; }
        public InvasionReward DefenderReward { get; set; }
        public InvasionMissionInfo DefenderMissionInfo { get; set; }
        public Activation Activation { get; set; }
    }

    public class InvasionReward
    {
        public List<CountedItem> CountedItems { get; set; }
    }

    public class InvasionMissionInfo
    {
        public int Seed { get; set; }
        public string Faction { get; set; }
        public List<object> MissionReward { get; set; }
    }

    #endregion

    #region Project

    public class ProjectsModel
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public SolidColorBrush Color { get; set; }
    }

    #endregion

    #region Global

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

    public class CountedItem
    {
        public string ItemType { get; set; }
        public int ItemCount { get; set; }
    }

    #endregion
}