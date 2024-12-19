namespace IAUI
{
    public static class UILocate
    {
        public static UICenter UICenter { get; private set; }

        public static UIServer UI { get; private set; }

        static UILocate()
        {
        }

        public static void Init()
        {
            UI = new UIServer();
            UI.Init();
        }

        public static void SetUICenter(UICenter uICenter)
        {
            UICenter = uICenter;
        }

        public static void Clear()
        {
            UICenter = null;
            UI = null;
        }
    }
}