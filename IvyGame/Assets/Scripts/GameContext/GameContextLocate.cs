using IAFramework;

namespace GameContext
{
    public static class GameContextLocate
    {
        public static PlayerData Player {  get; private set; }

        public static SettingData Setting {  get; private set; }


        public static void Init()
        {
            Player = ArchiveHelper.LoadRecord<PlayerData>("PlayerData");
            Setting = ArchiveHelper.LoadRecord<SettingData>("SettingData");

            GameEnv.Audio.GlobalVolume = Setting.Audio.GlobalVolume;
            GameEnv.Audio.BGVolume = Setting.Audio.BGVolume;
            GameEnv.Audio.AudioVolume = Setting.Audio.AudioVolume;
            GameEnv.Audio.IsMute = Setting.Audio.IsMute;
        }

        public static void SaveRecord()
        {
            ArchiveHelper.SaveRecord("PlayerData", Player);
            ArchiveHelper.SaveRecord("SettingData", Setting);
        }

        public static void Clear()
        {

        }

        public static void DelRecord()
        {
            ArchiveHelper.DelRecord("PlayerData");
            ArchiveHelper.DelRecord("SettingData");

            Player = new PlayerData();
            Setting = new SettingData();
        }
    }
}
