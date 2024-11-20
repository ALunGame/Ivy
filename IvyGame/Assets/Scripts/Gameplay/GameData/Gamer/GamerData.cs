using Game.Network;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameData
{
    public class GamerData : BaseGameData
    {
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
        /// 上次玩家位置
        /// </summary>
        public Vector2 LastPosition { get; protected set; }

        /// <summary>
        /// 玩家网格位置
        /// </summary>
        public GameDataField<Vector2Int> GridPos { get; protected set; }

        /// <summary>
        /// 玩家旋转
        /// </summary>
        public float Rotation { get; protected set; }

        /// <summary>
        /// 上次玩家旋转
        /// </summary>
        public float LastRotation { get; protected set; }

        /// <summary>
        /// 玩家移动速度
        /// </summary>
        public float MoveSpeed { get; protected set; }

        /// <summary>
        /// 玩家存活状态
        /// </summary>
        public GameDataField<bool> IsAlive { get; protected set; }

        /// <summary>
        /// 复活时长
        /// </summary>
        public GameDataField<float> RebornTime { get; protected set; }

        /// <summary>
        /// 击杀数
        /// </summary>
        public GameDataField<int> KillCnt { get; protected set; }

        /// <summary>
        /// 死亡数
        /// </summary>
        public GameDataField<int> DieCnt { get; protected set; }

        /// <summary>
        /// 鼓点音乐Id
        /// </summary>
        public int DrumsMusicId { get; protected set; }

        /// <summary>
        /// 鼓点间隔时间
        /// </summary>
        public float DrumsTime { get; protected set; }

        public GamerData(GamerInfo pInfo)
        {
            GamerUid = pInfo.Uid;
            GamerId = pInfo.Id;
            Name = pInfo.Name;
            Camp = pInfo.Camp;

            Position = pInfo.Pos.ToVector2();

            Rotation = pInfo.Rotation;
            MoveSpeed = pInfo.moveSpeed;

            SetDrumsMusicId(pInfo.fightMusicId);

            GridPos = new GameDataField<Vector2Int>(this);
            GridPos.SetValueWithoutNotify(GameplayGlobal.Data.Map.PosToGrid(Position));

            IsAlive = new GameDataField<bool>(this);
            IsAlive.SetValueWithoutNotify(true);

            RebornTime = new GameDataField<float>(this);
            RebornTime.SetValueWithoutNotify(0);

            KillCnt = new GameDataField<int>(this);
            KillCnt.SetValueWithoutNotify(0);

            DieCnt = new GameDataField<int>(this);
            DieCnt.SetValueWithoutNotify(0);
        }

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            base.UpdateLogic(pTimeDelta, pGameTime);
        }

        #region Set

        public void SetDrumsMusicId(int pMusicId)
        {
            FightDrumsMusicCfg cfg = IAConfig.Config.FightDrumsMusicCfg[pMusicId];
            DrumsMusicId = pMusicId;
            DrumsTime = cfg.drumsTime;
        }


        #endregion

        #region 玩家状态

        /// <summary>
        /// 死亡
        /// </summary>
        public void Die(GamerDieInfo pInfo)
        {
            IsAlive.Value = false;
            RebornTime.Value = pInfo.rebornTime;
            DieCnt.Value++;
        }

        /// <summary>
        /// 击杀
        /// </summary>
        public void Kill(GamerDieInfo pInfo)
        {
            KillCnt.Value++;
        }

        /// <summary>
        /// 重生
        /// </summary>
        public void Reborn(GamerRebornS2c pInfo)
        {
            IsAlive.Value = true;
            Position = pInfo.Pos.ToVector2();
        }

        #endregion

        #region 路径点

        public Dictionary<int, List<int>> PathDict { get; protected set; } = new Dictionary<int, List<int>>();

        public Action<Vector2Int> OnAddPathPoint;
        public Action<Vector2Int> OnRemovePathPoint;
        public Action OnClearPath;

        public void AppPathPoint(Vector2Int pPos)
        {
            if (PathDict.ContainsKey(pPos.x) && PathDict[pPos.x].Contains(pPos.y))
            {
                Debug.LogError($"AppPathPoint错误:{pPos}");
                return;
            }
            if (!PathDict.ContainsKey(pPos.x))
                PathDict.Add(pPos.x, new List<int>());
            PathDict[pPos.x].Add(pPos.y);

            OnAddPathPoint?.Invoke(pPos);
        }

        public void RemovePathPoint(Vector2Int pPos)
        {
            if (!PathDict.ContainsKey(pPos.x) || !PathDict[pPos.x].Contains(pPos.y))
            {
                Debug.LogError($"RemovePathPoint错误:{pPos}");
                return;
            }

            Debug.LogWarning($"RemovePathPoint成功:{pPos}");
            PathDict[pPos.x].Remove(pPos.y);
            OnRemovePathPoint?.Invoke(pPos);
        }

        public void ClearPath()
        {
            PathDict.Clear();
            OnClearPath?.Invoke();
        } 

        #endregion

        public virtual void OnReceiveServerStateMsg(ServerStateS2c pServerStateMsg, GamerBaseState pMsg)
        {
            LastPosition = Position;
            LastRotation = Rotation;

            //同步
            Position = pMsg.Pos.ToVector2();
            Rotation = pMsg.Rotation;
        }

        public void ForceSetPos(NetVector2 pPos)
        {
            Position = pPos.ToVector2();
            LastPosition = Position;
        }
    }
}
