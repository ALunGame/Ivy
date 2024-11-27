using Game;
using Game.Network;
using Gameplay.GameData;
using IAEngine;
using IAFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public abstract class Actor_InternalGamer : ActorModel
    {
        public float Speed { get; private set; }

        public int Camp { get; private set; }

        public ActorModelDataFile<Vector2Int> GridPos { get; private set; }

        public Dictionary<int, Dictionary<int, MapGamerPath>> PathPoint = new Dictionary<int, Dictionary<int, MapGamerPath>>();

        public Actor_InternalGamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo) { }

        protected override void OnInit()
        {
            GridPos = new ActorModelDataFile<Vector2Int>(this);
        }

        protected override void OnPosChange()
        {
            Vector3 tPos = GetPos();
            Vector2Int tGridPos = GameplayGlobal.Data.Map.PosToGrid(new Vector2(tPos.x, tPos.z));
            GridPos.Value = tGridPos;
        }

        public void SetSpeed(float pSpeed)
        {
            Speed = pSpeed;
        }

        public void SetCamp(int pCamp)
        {
            Camp = pCamp;
        }

        public void SetGamerData(GamerData pData)
        {
            SetCamp(pData.Camp);
            OnSetGamerData(pData);
        }

        protected abstract void OnSetGamerData(GamerData pData);

    }

    /// <summary>
    /// 一局游戏中所有的玩家，参与者
    /// </summary>
    public abstract class Actor_Gamer<T> : Actor_InternalGamer where T : GamerData
    {
        public const string PathPointCachePoolName = "Actor_GamerPathPoint";

        public T Data { get; protected set; }

        private float preMoveTotalTime = NetworkLogicTimer.FixedDelta * 3;
        private Vector2 currTargetPos;
        private Vector3 currMoveTargetPos;
        private float currMoveTime;

        protected Actor_Gamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
        }

        #region 生命周期

        public override void UpdateLogic(float pTimeDelta, float pGameTime)
        {
            SyncPosAndRotation();

            foreach (var item in PathPoint.Values)
            {
                foreach (var pathCom in item.Values)
                {
                    pathCom.UpdateLogic(pTimeDelta, pGameTime);
                }
            }
        }

        protected override void OnClear() 
        {
            RemoveDataChangeEvent();
            OnClear();
        }

        #endregion

        #region 数据监听

        public void RegDataChangeEvent()
        {
            Data.OnAddPathPoint += OnPathPointAdd;
            Data.OnRemovePathPoint += OnPathPointRemove;
            Data.OnClearPath += OnPathClear;

            Data.IsAlive.RegValueChangedEvent(OnAliveStateChange);

            Data.OnForceSetPos += SyncForceSetPos;

            OnRegDataChangeEvent();
        }
        protected virtual void OnRegDataChangeEvent() { }

        public void RemoveDataChangeEvent()
        {
            Data.OnAddPathPoint -= OnPathPointAdd;
            Data.OnRemovePathPoint -= OnPathPointRemove;
            Data.OnClearPath -= OnPathClear;

            Data.IsAlive.RemoveValueChangedEvent(OnAliveStateChange);

            Data.OnForceSetPos -= SyncForceSetPos;

            OnRemoveDataChangeEvent();
        }
        protected virtual void OnRemoveDataChangeEvent() { }

        #endregion

        #region 路径点改变

        private int currPathCnt = 0;
        protected virtual void OnPathPointAdd(Vector2Int pPos)
        {
            if (!CachePool.HasGameObjectPool(PathPointCachePoolName))
            {
                CachePool.CreateGameObjectPool(PathPointCachePoolName, () =>
                {
                    return GameEnv.Asset.CreateGo("GamerPath");
                });
            }

            Transform pathRootTrans = null;
            if (!GameplayGlobal.Map.MapTrans.Find($"{Uid}_Path", out pathRootTrans))
            {
                pathRootTrans = new GameObject($"{Uid}_Path").transform;
                pathRootTrans.SetParent(GameplayGlobal.Map.MapTrans);
                pathRootTrans.Reset();
            }

            GameObject pathPointGo = CachePool.GetGameObject(PathPointCachePoolName);
            pathPointGo.transform.SetParent(pathRootTrans);
            pathPointGo.transform.position = new Vector3(pPos.x, 0.1f, pPos.y);
            pathPointGo.name = pPos.ToString();

            MeshRenderColorCom meshRenderColorCom = pathPointGo.transform.Find("Display/Cube").GetComponent<MeshRenderColorCom>();
            if (TempConfig.CampColorDict[Camp] == Color.white)
            {
                Debug.LogError($"Camp--->{Camp}");
            }
            meshRenderColorCom.ChangeColor(TempConfig.CampColorDict[Camp]);

            if (!PathPoint.ContainsKey(pPos.x))
                PathPoint.Add(pPos.x, new Dictionary<int, MapGamerPath>());
            
            if (PathPoint[pPos.x].ContainsKey(pPos.y))
            {
                Debug.LogError($"OnPathPointAdd--->{pPos}");
                return;
            }

            MapGamerPath mapGamerPath = pathPointGo.GetComponent<MapGamerPath>();

            currPathCnt++;
            PathPoint[pPos.x].Add(pPos.y, mapGamerPath);

            if (currPathCnt % 5 == 0)
            {
                mapGamerPath.SetAnimCfg(new Vector2(1.5f, 2.5f));
            }
            else
            {
                mapGamerPath.SetAnimCfg(new Vector2(0.1f, 0.1f));
            }
        }

        protected virtual void OnPathPointRemove(Vector2Int pPos)
        {
            if (PathPoint.ContainsKey(pPos.x) && PathPoint[pPos.x].ContainsKey(pPos.y))
            {
                currPathCnt--;
                MapGamerPath mapGamerPath = PathPoint[pPos.x][pPos.y];
                PathPoint[pPos.x].Remove(pPos.y);

                CachePool.PushGameObject(PathPointCachePoolName, mapGamerPath.gameObject);
            }
            else
            {
                Debug.LogError($"OnPathPointRemove--->错误::{pPos}");
            }
        }

        protected virtual void OnPathClear()
        {
            foreach (var item in PathPoint.Values)
            {
                foreach (var pathCom in item.Values)
                {
                    CachePool.PushGameObject(PathPointCachePoolName, pathCom.gameObject);
                }
            }
            PathPoint.Clear();
            currPathCnt = 0;
        }

        #endregion

        #region 玩家状态改变

        private void OnAliveStateChange(bool pIsAlive, bool pOldValue)
        {
            if (pIsAlive)
            {
                OnReborn();
            }
            else 
            {
                OnDie();
            }
        }

        public virtual void OnDie()
        {
            SetActive(false);
        }

        public virtual void OnReborn()
        {
            SetActive(true);
        }

        #endregion

        #region 同步

        private void SyncForceSetPos(Vector2 pPos)
        {
            currMoveTime = 0;
            currTargetPos = new Vector2(pPos.x, pPos.y);
            currMoveTargetPos = new Vector3(currTargetPos.x, GetPos().y, currTargetPos.y);
            SetPos(currTargetPos);
        }

        private void SyncPosAndRotation()
        {
            if (!Data.Position.Equals(currTargetPos))
            {
                currMoveTime = 0;
                currTargetPos = Data.Position;
                currMoveTargetPos = new Vector3(currTargetPos.x, GetPos().y, currTargetPos.y);
            }

            Vector3 movePos = Vector3.Lerp(GetPos(), currMoveTargetPos, currMoveTime / preMoveTotalTime);
            currMoveTime += NetworkLogicTimer.FixedDelta;

            SetPos(movePos);
            SetRotation(Quaternion.Euler(0, Data.Rotation, 0));
        }

        #endregion

        protected sealed override void OnSetGamerData(GamerData pData)
        {
            Data = (T)pData;
            RegDataChangeEvent();
        }
    }
}
