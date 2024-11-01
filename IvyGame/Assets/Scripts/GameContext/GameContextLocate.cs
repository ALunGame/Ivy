namespace GameContext
{
    internal static class GameContextLocate
    {
        public static PlayerData Player {  get; private set; }


        public static void Init()
        {
            Player = ArchiveHelper.LoadRecord<PlayerData>("PlayerData");
        }

        public static void SaveRecord()
        {
            ArchiveHelper.SaveRecord("PlayerData", Player);
        }

        public static void Clear()
        {

        }

        public static void DelRecord()
        {
            ArchiveHelper.DelRecord("PlayerData");
            Player = new PlayerData();
        }
    }
}
