namespace Game.Network.Server
{
    internal class AddServerPlayerInfo
    {
        public int uid;
        public string name;
        public byte camp;

        public AddServerPlayerInfo(int pUid, string pName, byte pCamp)
        {
            uid = pUid;
            name = pName;
            camp = pCamp;
        }
    }

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
        /// 击杀次数
        /// </summary>
        public int KillCnt { get; set; }

        /// <summary>
        /// 死亡次数
        /// </summary>
        public int DieCnt { get; set; }

        /// <summary>
        /// 玩家状态
        /// </summary>
        public PlayerState State { get; private set; }

        /// <summary>
        /// 上一次位置
        /// </summary>
        public ServerPoint LastPos { get; private set; }

        /// <summary>
        /// 位置
        /// </summary>
        public ServerPoint Pos { get; private set; }

        public ServerPlayer(AddServerPlayerInfo pInfo) : this(pInfo.uid, pInfo.name, pInfo.camp)
        {

        }

        public ServerPlayer(int uid, string name, byte camp)
        {
            Uid = uid;
            Name = name;
            Camp = camp;
            State = PlayerState.Alive;
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

        public void Die()
        {
            if (State == PlayerState.Die)
            {
                return;
            }
            State = PlayerState.Die;
        }

        public void Reborn()
        {
            if (State == PlayerState.Alive)
            {
                return;
            }

            State = PlayerState.Alive;
        }
    }
}
