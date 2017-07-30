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

    public class GameSnapshotModel
    {
        public int Version { get; set; }
        public string MobileVersion { get; set; }
        public string BuildLabel { get; set; }
        public int Time { get; set; }
        public int Date { get; set; }
        public Alert[] Alerts { get; set; }
        public Invasion[] Invasions { get; set; }
        [JsonConverter(typeof(ProjectConverter))]
        public ProjectsModel[] ProjectPct { get; set; }
        public string WorldSeed { get; set; }
    }

    #endregion

    #region Alert

    public class Alert : NotifyPropertyChangedImpl
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        public Activation Activation { get; set; }
        public Expiry Expiry { get; set; }
        public MissionInfo MissionInfo { get; set; }
        private Brush _statusColor;

        public Brush StatusColor
        {
            get => _statusColor;
            set => Set(ref _statusColor, value);
        }
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
                    StatusColor = Brushes.Orange;
                }
                else
                {
                    if (DateTime.Now <= end)
                    {
                        value = (end - DateTime.Now).ToString((end - DateTime.Now.TimeOfDay).Hour == 0
                            ? @"mm\:ss"
                            : @"hh\:mm\:ss");
                        StatusColor = (SolidColorBrush) new BrushConverter().ConvertFrom("#6ECD37");
                    }
                    else
                    {
                        value = "00:00";
                        StatusColor = Brushes.Red;
                    }
                }
                Set(ref _status, value);
            }
        }
    }

    public class MissionInfo : VM
    {
        public string MissionType { get; set; }
        public string Faction { get; set; }
        public string[] Planet { get; set; }
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
        
        private string _reward;
        public string Reward
        {
            get => _reward;
            set => Set(ref _reward, value);
        }

        private Brush _rewardColor;

        public Brush RewardColor
        {
            get => _rewardColor;
            set => Set(ref _rewardColor, value);
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

        public Visibility CreditVisibility => MissionReward.Credits > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public class MissionReward
    {
        public int Credits { get; set; }
        public List<CountedItem> CountedItems { get; set; }
        public List<string> Items { get; set; }
    }

    #endregion

    #region Invasions

    public class Invasion: NotifyPropertyChangedImpl
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        public string Faction { get; set; }
        public string Node { get; set; }
        public string[] NodeArray { get; set; }

        private double _count;
        public double Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        private double _goal;
        public double Goal
        {
            get => _goal;
            set => Set(ref _goal, value);
        }

        private double _percentOut;
        public double PercentOut
        {
            get => _percentOut;
            set => Set(ref _percentOut, value);
        }

        private double _percent;
        public double Percent
        {
            get
            {
                var val = DefenderMissionInfo.Faction == "FC_INFESTATION"
                    ? (Goal + Count) / Goal * 100
                    : (Goal + Count) / (Goal * 2) * 100;

                PercentOut = 100 - val;
                _percent = val;
                return _percent;
            }
            set
            {
                Set(ref _percent, value);
            }
        }

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

    public class InvasionMissionInfo: VM
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