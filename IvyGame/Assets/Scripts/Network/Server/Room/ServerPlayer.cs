namespace Game.Network.Server
{
    internal class ServerPlayer
    {
        /// <summary>
        /// 玩家Uid
        /// </summary>
        public int Uid {  get; private set; }

        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 阵营Id
        /// </summary>
        public int Camp { get; private set; }

        public int PosX { get; private set; }
        public int PosY { get; private set; }

        public ServerPlayer(int uid, string name, int camp)
        {
            Uid = uid;
            Name = name;
            Camp = camp;

            PosX = 0;
            PosY = 0;
        }

        public void UpdatePos(int posX, int posY) 
        {
            PosX = posX;
            PosY = posY;
        }
    }
}
