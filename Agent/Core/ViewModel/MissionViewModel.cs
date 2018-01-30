using System;
using System.Windows;

using Core.Model;
using Core.Events;

namespace Core.ViewModel
{
    public class MissionViewModel : VM
    {
        private string missionType;
        private SectorViewModel sector;
        MissionInfo missionInfo;

        public int MinEnemyLevel { get; }
        public int MaxEnemyLevel { get; }
        public FactionViewModel Faction { get; }
        public SectorViewModel Sector { get => sector; private set => Set(ref sector, value);}
        public string MissionType { get => missionType; private set => Set(ref missionType, value); }
        public Visibility ModeVisibility { get; }
        public Visibility NightmareVisibility { get; }
        public string ModeIcon { get; }
        public string ModeToolTip { get; }
        public MissionRewardViewModel Reward { get; }

        public MissionViewModel(MissionInfo missionInfo, FiltersEvent filtersEvent)
        {
            Reward = new MissionRewardViewModel(missionInfo.MissionReward, filtersEvent);
            Faction = FactionViewModel.ById(missionInfo.Faction);
            Sector = SectorViewModel.FromSector(missionInfo.Location);
            MissionType = Model.Filters.ExpandMission(missionInfo.MissionType)?.Name ?? missionInfo.MissionType;

            if (missionInfo.ArchwingRequired == true || missionInfo.IsSharkwingMission == false)
            {
                ModeIcon = "PaperPlaneOutline";
                ModeToolTip = "Арчвинг миссия";
                ModeVisibility = Visibility.Visible;
            }
            else if (missionInfo.IsSharkwingMission == true)
            {
                ModeIcon = "SnowflakeOutline";
                ModeToolTip = "Шарквинг миссия";
                ModeVisibility = Visibility.Visible;
            }

            NightmareVisibility = missionInfo.Nightmare == true
                ? Visibility.Visible
                : Visibility.Collapsed;

            MinEnemyLevel = missionInfo.MinEnemyLevel;
            MaxEnemyLevel = missionInfo.MaxEnemyLevel;

            this.missionInfo = missionInfo;
            MissionsUpdatedWeakEventManager.AddHandler(filtersEvent, OnMissionsFilterUpdated);
            SectorsUpdatedWeakEventManager.AddHandler(filtersEvent, OnSectorsFilterUpdated);
        }

        private void OnMissionsFilterUpdated(object sender, EventArgs eventArgs)
        {
            MissionType = Model.Filters.ExpandMission(missionInfo.MissionType)?.Name ?? missionInfo.MissionType;
        }

        private void OnSectorsFilterUpdated(object sender, EventArgs eventArgs)
        {
            Sector = SectorViewModel.FromSector(missionInfo.Location);
        }
    }
}
