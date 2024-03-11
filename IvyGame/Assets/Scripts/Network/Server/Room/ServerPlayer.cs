using System;

namespace Game.Network.Server
{
    internal class AddServerPlayerInfo
    {
        public int uid;
        public int id;
        public string name;
        public byte camp;

        public AddServerPlayerInfo(int pUid, int pId, string pName, byte pCamp)
        {
            uid = pUid;
            id = pId;
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
            if (HorDir == pHorDir && VerDir == pVerDir)
            {
                return false;
            }
            HorDir = pHorDir;
            VerDir = pVerDir;
            return false;
        }

        public ServerPos CalcMovePos(ServerPos currPos, float moveDel)
        {
            float xDel = HorDir * moveDel + currPos.x;
            float yDel = VerDir * moveDel + currPos.y;
            return new ServerPos(xDel, yDel);
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
        /// 玩家配置Id
        /// </summary>
        public int Id { get; private set; }

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
        /// 移动方向
        /// </summary>
        public PlayerMoveDir MoveDir { get; private set; }

        /// <summary>
        /// 上一个网格点位置
        /// </summary>
        public ServerPoint LastGridPos { get; private set; }

        /// <summary>
        /// 网格点位置
        /// </summary>
        public ServerPoint GridPos { get; private set; }

        /// <summary>
        /// 世界坐标
        /// </summary>
        public ServerPos Pos { get; private set; }

        /// <summary>
        /// 复活时间
        /// </summary>
        public float RebornTime { get; set; }

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
            LastGridPos = new ServerPoint();
            Pos = new ServerPos();
        }

        public void SetCamp(byte camp)
        {
            Camp = camp;
        }

        public void SetGridPos(byte posX, byte posY) 
        {
            if (GridPos != null)
            {
                LastGridPos.x = GridPos.x;
                LastGridPos.y = GridPos.y;
            }
            else
            {
                GridPos = new ServerPoint();
            }

            GridPos.x = posX;
            GridPos.y = posY;
        }

        public void SetPos(float posX, float posY)
        {
            posX = posX < 0 ? 0 : posX;
            posY = posY < 0 ? 0 : posY; 

            Pos.x = posX;
            Pos.y = posY;
        }

        public byte ToGridPos(float pos)
        {
            if (pos < 0)
                return byte.MaxValue;

            return (byte)MathF.Floor(pos);
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
            MoveDir.ChangeDir(pHorDir, pVerDir);
        }

        public void Die()
        {
            if (State == PlayerState.Die)
            {
                return;
            }
            State = PlayerState.Die;
            RebornTime = TempConfig.RebornTime;
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
