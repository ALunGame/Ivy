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

    internal class PlayerMoveDir
    {
        public int HorDir;
        public int VerDir;

        public bool ChangeDir(int pHorDir, int pVerDir)
        {
            if (HorDir == pHorDir || VerDir == pVerDir)
            {
                return false;
            }
            if (HorDir != pHorDir)
            {
                HorDir = pHorDir;
                return true;
            }
            else
            {
                VerDir = pVerDir;
                return true;
            }
        }
        
        public int GetValue()
        {
            if (HorDir != 0)
            {
                return HorDir;
            }
            else
            {
                return VerDir;
            }
        }

        public void Reset()
        {
            HorDir = 0;
            VerDir = 0;
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
        /// 玩家速度
        /// </summary>
        public float Speed { get; private set; }

        /// <summary>
        /// 移动缓存
        /// </summary>
        public float MoveDelCache { get; set; }

        /// <summary>
        /// 移动方向
        /// </summary>
        public PlayerMoveDir MoveDir { get; private set; }

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
            Speed = 0;
            MoveDir = new PlayerMoveDir();
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

        public void UpdateSpeed(float pAddSpeed)
        {
            if (State != PlayerState.Alive)
            {
                return;
            }
            if (Speed == 0)
            {
                Speed = NetworkGeneral.BaseSpeed;
            }
            Speed += pAddSpeed;
        }

        public void ChangeMoveDir(int pHorDir, int pVerDir)
        {
            if (MoveDir.ChangeDir(pHorDir,pVerDir))
            {
                MoveDelCache = 0;
            }
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
