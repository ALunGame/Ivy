namespace LCUI
{
    public static class UILocate
    {
        public static UICenter UICenter { get; private set; }

        public static UIServer UI { get; private set; }

        public static UILogServer Log { get; private set; }

        public static void Init()
        {
            UI = new UIServer();
            UI.Init();
            Log = new UILogServer();
        }

        public static void SetUICenter(UICenter uICenter)
        {
            UICenter = uICenter;
        }

        public static void Clear()
        {
            UICenter = null;
            UI = null;
            Log = null;
        }
    }
}