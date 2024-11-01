using Game;
using Gameplay.GameData;
using IAEngine;
using IAFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public abstract class Actor_InternalGamer : ActorModel
    {
        public Dictionary<int, Dictionary<int, GameObject>> PathPoint = new Dictionary<int, Dictionary<int, GameObject>>();

        public float Speed { get; private set; }

        public int Camp { get; private set; }

        public Actor_InternalGamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {

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

        protected T Data;

        protected Actor_Gamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
        }

        #region 生命周期

        public sealed override void Init()
        {
            OnInit();
        }
        protected virtual void OnInit() { }

        public sealed override void Clear()
        {
            RemoveDataChangeEvent();
            OnClear();
        }
        protected virtual void OnClear() { }

        #endregion

        #region 数据监听

        public void RegDataChangeEvent()
        {
            Data.OnAddPathPoint += OnPathPointAdd;
            Data.OnRemovePathPoint += OnPathPointRemove;
            Data.OnClearPath += OnPathClear;

            Data.IsAlive.RegValueChangedEvent(OnAliveStateChange);

            OnRegDataChangeEvent();
        }
        protected virtual void OnRegDataChangeEvent() { }

        public void RemoveDataChangeEvent()
        {
            Data.OnAddPathPoint -= OnPathPointAdd;
            Data.OnRemovePathPoint -= OnPathPointRemove;
            Data.OnClearPath -= OnPathClear;

            Data.IsAlive.RemoveValueChangedEvent(OnAliveStateChange);

            OnRemoveDataChangeEvent();
        }
        protected virtual void OnRemoveDataChangeEvent() { }

        #endregion

        #region 路径点改变

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

            MeshRenderColorCom meshRenderColorCom = pathPointGo.transform.Find("Display/Cube").GetComponent<MeshRenderColorCom>();
            meshRenderColorCom.ChangeColor(TempConfig.CampColorDict[Camp]);
        }

        protected virtual void OnPathPointRemove(Vector2Int pPos)
        {
            if (PathPoint.ContainsKey(pPos.x) && PathPoint[pPos.x].ContainsKey(pPos.y))
            {
                GameObject pathPointGo = PathPoint[pPos.x][pPos.y];
                PathPoint[pPos.x].Remove(pPos.x);

                CachePool.PushGameObject(PathPointCachePoolName,pathPointGo);
            }
        }

        protected virtual void OnPathClear()
        {
            foreach (var item in PathPoint.Values)
            {
                foreach (var go in item.Values)
                {
                    CachePool.PushGameObject(PathPointCachePoolName, go);
                }
            }
            PathPoint.Clear();
        }

        #endregion

        #region 玩家状态改变

        private void OnAliveStateChange(bool pIsAlive)
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

        protected sealed override void OnSetGamerData(GamerData pData)
        {
            Data = (T)pData;
            RegDataChangeEvent();
        }
    }
}
