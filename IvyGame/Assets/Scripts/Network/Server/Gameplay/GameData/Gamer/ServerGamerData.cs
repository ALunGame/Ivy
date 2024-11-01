﻿using Gameplay;
using IAConfig;
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
        /// 鼓点音乐Id
        /// </summary>
        public int DrumsMusicId { get; protected set; }

        /// <summary>
        /// 鼓点间隔时间
        /// </summary>
        public float DrumsTime { get; protected set; }

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
        /// 玩家移动速度
        /// </summary>
        public float MoveSpeed { get; protected set; }
        public Vector2 MoveInputDir { get; protected set; }
        private MoveClickType buffAddSpeedType;
        private float buffAddSpeed;
        private float buffAddSpeedTime;

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

        public ServerGamerData(NetPeer pPeer, string pGamerUid, int pGamerId, string pName)
        {
            Peer = pPeer;
            GamerUid = pGamerUid;
            GamerId = pGamerId;
            Name = pName;

            MoveInputDir = Vector2.zero;
            GridPos = new ServerGameDataFile<Vector2Int>(this);

            IsAlive = new ServerGameDataFile<bool>(this);
            IsAlive.SetValueWithoutNotify(true);

            RebornTime = new ServerGameDataFile<float>(this);
            RebornTime.SetValueWithoutNotify(0);
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

            //更新速度
            if (buffAddSpeedTime > 0)
            {
                buffAddSpeedTime -= pTimeDelta;

                if (buffAddSpeedTime <= 0)
                {
                    //MoveSpeed -= buffAddSpeed;
                }
            }

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

        public virtual void OnRecInputMsg(GamerInputC2s pMsg)
        {
            //if (NetworkGeneral.SeqDiff(pMsg.commandTick, LastCommandTick) <= 0)
            //    return;
            //LastCommandTick = pMsg.commandTick;

            Vector2 velocity = Vector2.zero;

            if (pMsg.commandType == PlayerInputCommand.Move_Up)
                velocity.y = -1f;
            if (pMsg.commandType == PlayerInputCommand.Move_Down)
                velocity.y = 1f;

            if (pMsg.commandType == PlayerInputCommand.Move_Left)
                velocity.x = -1f;
            if (pMsg.commandType == PlayerInputCommand.Move_Right)
                velocity.x = 1f;
            MoveInputDir = velocity;

            HandleInputMoveSpeedChange();

            Rotation = pMsg.Rotation;
        }

        private void HandleInputMoveSpeedChange()
        {
            float offsetTime = NetServerLocate.GameCtrl.GameTime % DrumsTime;
            buffAddSpeedType = MoveClickType.Miss;

            //普通
            if (0 < offsetTime && offsetTime < DrumsTime * 0.2f)
            {
                buffAddSpeed = 2;
                buffAddSpeedTime = 1;
                buffAddSpeedType = MoveClickType.Normal;
            }
            //优秀
            else if (DrumsTime * 0.2f <= offsetTime && offsetTime < DrumsTime * 0.8f)
            {
                buffAddSpeed = 4;
                buffAddSpeedTime = 2;
                buffAddSpeedType = MoveClickType.Good;
            }
            //完美
            else if (DrumsTime * 0.8f <= offsetTime && offsetTime <= DrumsTime)
            {
                buffAddSpeed = 6;
                buffAddSpeedTime = 3;
                buffAddSpeedType = MoveClickType.Perfect;
            }
            //失误
            else 
            {
                buffAddSpeed = 0;
                buffAddSpeedTime = 0;
                buffAddSpeedType = MoveClickType.Miss;
            }
        }

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
    }
}
