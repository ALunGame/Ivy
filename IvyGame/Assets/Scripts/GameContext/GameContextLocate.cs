namespace GameContext
{
    internal static class GameContextLocate
    {
        public static PlayerData Player {  get; private set; }


        public static void Init()
        {
            Player = RecordData.LoadRecord<PlayerData>("PlayerData");
        }

        public static void SaveRecord()
        {
            RecordData.SaveRecord("PlayerData", Player);
        }

        public static void Clear()
        {

        }
    }
}
