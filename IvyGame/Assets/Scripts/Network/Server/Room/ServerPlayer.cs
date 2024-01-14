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
        public byte Camp { get; private set; }

        /// <summary>
        /// 上一次位置
        /// </summary>
        public ServerPoint LastPos { get; private set; }

        /// <summary>
        /// 位置
        /// </summary>
        public ServerPoint Pos { get; private set; }

        public ServerPlayer(int uid, string name, byte camp)
        {
            Uid = uid;
            Name = name;
            Camp = camp;

            Pos = new ServerPoint();
        }

        public void SetPos(byte posX, byte posY) 
        {
            if (LastPos == null)
            {
                LastPos = new ServerPoint(false);
            }
            else
            {
                LastPos.isLegal = true;
                LastPos.x = Pos.x;
                LastPos.y = Pos.y;
            }
            Pos.x = posX;
            Pos.y = posY;
        }
    }
}
