using Gameplay;
using Gameplay.Map;
using IAConfig;
using IAEngine;
using LiteNetLib;
using Proto;
using UnityEngine;

namespace Game.Network.Server
{
    internal class ServerGamerData : BaseServerGameData
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        public NetPeer Peer { get; private set; }

        /// <summary>
        /// 玩家Uid
        /// </summary>
        public string GamerUid { get; protected set; }

        /// <summary>
        /// 玩家Id
        /// </summary>
        public int GamerId { get; protected set; }

        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 玩家阵营
        /// </summary>
        public int Camp { get; protected set; }

        /// <summary>
        /// 玩家位置
        /// </summary>
        public Vector2 Position { get; protected set; }

        /// <summary>
        /// 上一次玩家格子位置
        /// </summary>
        public Vector2Int LastGridPos { get; protected set; }

        /// <summary>
        /// 玩家格子位置
        /// </summary>
        public ServerGameDataFile<Vector2Int> GridPos { get; protected set; }

        /// <summary>
        /// 玩家存活状态
        /// </summary>
        public ServerGameDataFile<bool> IsAlive { get; protected set; }

        /// <summary>
        /// 复活时长
        /// </summary>
        public ServerGameDataFile<float> RebornTime { get; protected set; }

        /// <summary>
        /// 玩家旋转
        /// </summary>
        public float Rotation { get; protected set; }

        /// <summary>
        /// 基础移动速度
        /// </summary>
        public float BaseMoveSpeed { get; protected set; }

        /// <summary>
        /// 当前玩家移动速度
        /// </summary>
        public float MoveSpeed { get; protected set; }
        public Vector2 MoveInputDir { get; protected set; }
        public Vector2 LastMoveInputDir { get; protected set; }

        //节奏加速
        private float buffAddSpeedTypeTimer;
        private MoveClickType buffAddSpeedType;
        private float buffAddSpeedTime;

        //冲刺Cd
        private TimerModel dashTimer;
        private int dashGridCount;
        private bool isDashInCd;

        /// <summary>
        /// 上一次处理的命令帧
        /// </summary>
        public int LastCommandTick { get; private set; }

        /// <summary>
        /// 击杀数
        /// </summary>
        public int KillCnt { get; protected set; }

        /// <summary>
        /// 死亡数
        /// </summary>
        public int DieCnt { get; protected set; }

        /// <summary>
        /// 鼓点音乐Id
        /// </summary>
        public int DrumsMusicId { get; protected set; }

        /// <summary>
        /// 鼓点间隔时间
        /// </summary>
        public float DrumsTime { get; protected set; }

        public ServerGamerData(NetPeer pPeer, string pGamerUid, int pGamerId, string pName)
        {
            Peer = pPeer;
            GamerUid = pGamerUid;
            GamerId = pGamerId;
            Name = pName;

            ActorCfg actorCfg = Config.ActorCfg[pGamerId];
            BaseMoveSpeed = actorCfg.baseSpeed;
            SetMoveSpeed(actorCfg.baseSpeed);

            MoveInputDir = Vector2.zero;
            GridPos = new ServerGameDataFile<Vector2Int>(this);

            IsAlive = new ServerGameDataFile<bool>(this);
            IsAlive.SetValueWithoutNotify(true);

            RebornTime = new ServerGameDataFile<float>(this);
            RebornTime.SetValueWithoutNotify(0);

            //冲刺配置
            dashTimer = new TimerModel(GetType(), int.Parse(Config.MiscCfg["DashCdTime"].value), () =>
            {
                isDashInCd = false;
            }, 1);
            dashGridCount = int.Parse(Config.MiscCfg["DashGridCnt"].value);
            isDashInCd = false;
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            //死亡复活
            if (!IsAlive.Value) 
            {
                RebornTime.Value -= pTimeDelta;
                if (RebornTime.Value <= 0)
                {
                    NetServerLocate.GameCtrl.GameMode.GamerReborn(GamerUid);
                }
                return;
            }

            //计算点击类型
            CalcCurrMoveClickType(pTimeDelta);

            //更新计时器
            dashTimer.Update(pTimeDelta);

            //更新速度
            if (buffAddSpeedTime > 0)
            {
                buffAddSpeedTime -= pTimeDelta;

                if (buffAddSpeedTime <= 0)
                {
                    SetMoveSpeed(BaseMoveSpeed);
                }
            }

            //更新位置
            if (!MoveInputDir.Equals(Vector2.zero))
            {
                Vector2 newPos = Position + (MoveInputDir.normalized * MoveSpeed * pTimeDelta);
                if (NetServerLocate.GameCtrl.GameData.Map.CheckPointIsLegal((int)newPos.x, (int)newPos.y))
                {
                    SetPos(newPos);
                }
            }
        }

        #region 玩家状态

        /// <summary>
        /// 死亡
        /// </summary>
        public void Die()
        {
            IsAlive.Value = false;
            RebornTime.Value = float.Parse(Config.MiscCfg["GamerRebornTime"].value);
            DieCnt++;
        }

        /// <summary>
        /// 击杀
        /// </summary>
        public void Kill()
        {
            KillCnt++;
        }

        /// <summary>
        /// 重生
        /// </summary>
        public void Reborn()
        {
            IsAlive.Value = true;

        }

        #endregion

        #region Set

        public void SetPos(Vector2 pPos)
        {
            Position = pPos;

            LastGridPos = GridPos.Value;

            Vector2Int gridPos = NetServerLocate.GameCtrl.GameData.Map.PosToGrid(Position);
            GridPos.Value = gridPos;
        }

        public void SetRotation(float pRotation)
        {
            Rotation = pRotation;
        }

        public void SetMoveSpeed(float pMoveSpeed)
        {
            MoveSpeed = pMoveSpeed;
        }

        public void SetCamp(int pCamp)
        {
            Camp = pCamp;
        }

        public void SetDrumsMusicId(int pMusicId)
        {
            FightDrumsMusicCfg cfg = IAConfig.Config.FightDrumsMusicCfg[pMusicId];
            DrumsMusicId = pMusicId;
            DrumsTime = cfg.drumsTime;
        }

        #endregion

        #region 数据收集

        public GamerInfo CollectGamerInfo()
        {
            GamerInfo info = new GamerInfo();
            info.Uid = GamerUid;
            info.Name = Name;
            info.Id = GamerId;
            info.Camp = Camp;
            info.Pos = Position.ToNetVector2();
            info.Rotation = (int)Rotation;
            info.moveSpeed = (int)MoveSpeed;
            info.fightMusicId = DrumsMusicId;
            return info;
        }

        public GamerBaseState CollectGamerState()
        {
            GamerBaseState info = new GamerBaseState();
            info.gamerUid = GamerUid;
            info.Pos = Position.ToNetVector2();
            info.Rotation = (int)Rotation;
            info.commandTick = LastCommandTick;
            return info;
        }

        #endregion

        //计算当前时间处于什么点击类型
        private void CalcCurrMoveClickType(float pTimeDelta)
        {
            if (buffAddSpeedTypeTimer > DrumsTime)
            {
                buffAddSpeedType = MoveClickType.Miss;
                buffAddSpeedTypeTimer = 0;
            }

            buffAddSpeedTypeTimer += pTimeDelta;
            float offsetTime = DrumsTime - buffAddSpeedTypeTimer;

            //普通
            if (0 < offsetTime && offsetTime < DrumsTime * 0.6f)
            {
                buffAddSpeedType = MoveClickType.Normal;
            }
            //优秀
            else if (DrumsTime * 0.6f <= offsetTime && offsetTime < DrumsTime * 0.8f)
            { 
                buffAddSpeedType = MoveClickType.Good;
            }
            //完美
            else if (DrumsTime * 0.8f <= offsetTime && offsetTime <= DrumsTime)
            {
                buffAddSpeedType = MoveClickType.Perfect;
            }
            //失误
            else
            {
                buffAddSpeedType = MoveClickType.Miss;
            }
        }

        #region 网络消息

        public void OnRecMoveInputMsg(GamerInputC2s pMsg)
        {
            Rotation = pMsg.Rotation;

            LastMoveInputDir = MoveInputDir;
            Vector2 velocity = Vector2.zero;

            if (pMsg.commandType == PlayerInputCommand.Move_Up)
                velocity.y = 1f;
            if (pMsg.commandType == PlayerInputCommand.Move_Down)
                velocity.y = -1f;

            if (pMsg.commandType == PlayerInputCommand.Move_Left)
                velocity.x = -1f;
            if (pMsg.commandType == PlayerInputCommand.Move_Right)
                velocity.x = 1f;
            MoveInputDir = velocity;

            //切换到水平移动
            if (LastMoveInputDir.x == 0 && MoveInputDir.x != 0)
            {
                Position = new Vector2(Position.x, (int)Position.y);
            }

            //切换到竖直移动
            if (LastMoveInputDir.y == 0 && MoveInputDir.y != 0)
            {
                Position = new Vector2((int)Position.x, Position.y);
            }

            HandleInputMoveSpeedChange();

            //发送移动结果
            GamerInputS2c msg = new GamerInputS2c();
            msg.gamerUid = GamerUid;
            msg.moveClickType = (int)buffAddSpeedType;
            NetServerLocate.Net.SendTo(Peer, (ushort)RoomMsgDefine.GamerInputS2c, msg);
        }

        private void HandleInputMoveSpeedChange()
        {
            MoveClickCfg moveClickCfg = TempConfig.MoveClickCfgDict[buffAddSpeedType];
            buffAddSpeedTime = moveClickCfg.buffTime;
            SetMoveSpeed(BaseMoveSpeed * moveClickCfg.addSpeedRate);

            Debug.LogWarning($"Input:{buffAddSpeedTypeTimer}-{DrumsTime}:{buffAddSpeedType}::{MoveSpeed}");
        }

        public void OnRecSkillInputMsg(GamerSkillInputC2s pMsg)
        {
            GamerSkillInputS2c sendMsg = new GamerSkillInputS2c();
            sendMsg.gamerUid = GamerUid;
            if (isDashInCd)
            {
                sendMsg.RetCode = 1;
                Debug.LogError($"冲刺Cd中:{GamerUid}");
            }
            else
            {
                sendMsg.RetCode = 0;

                //0，设置Cd
                isDashInCd = true;
                //1，开启计时器
                dashTimer.Start();
                //2，瞬移
                Vector2Int dashDir = new Vector2Int((int)MoveInputDir.x, (int)MoveInputDir.y);
                if (dashDir == Vector2Int.zero)
                {
                    dashDir = new Vector2Int(1, 0);
                }
                Debug.LogError($"当前格子位置:{GridPos.Value}");

                for (int i = 0; i < dashGridCount; i++)
                {
                    Vector2Int newGridPos = GridPos.Value + dashDir;
                    if (NetServerLocate.GameCtrl.GameData.Map.CheckPointIsLegal(newGridPos.x, newGridPos.y))
                    {
                        GridPos.Value = newGridPos;
                        Debug.LogError($"瞬移格子:{GridPos.Value}");
                    }
                }
                Position = new Vector2(GridPos.Value.x, GridPos.Value.y);
                sendMsg.Pos = Position.ToNetVector2();
                Debug.LogError($"瞬移位置:{Position}");

                //测试
                //MoveInputDir = Vector2.zero;
            }

            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.GamerSkillInputS2c, sendMsg);
        }

        #endregion


    }
}
