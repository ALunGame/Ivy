using Gameplay.GameMap.Actor;
using Gameplay.Map;
using IAConfig;
using IAEngine;
using IAFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.GameMap.System
{
    public class GameActorSystem : BaseGameMapSystem
    {
        public Transform ActorRoot { get; private set; }
        public Dictionary<ActorType, List<ActorModel>> ActorDict { get; private set; }
        public List<ActorModel> ActorList { get; private set; }
        public List<ActorModel> UpdateActorList { get; private set; }

        /// <summary>
        /// 玩家
        /// </summary>
        public Actor_LocalGamer Player { get; private set; }

        public override void Init(GameMapCtrl pMapCtrl)
        {
            base.Init(pMapCtrl);

            ActorDict = new Dictionary<ActorType, List<ActorModel>>();
            ActorList = new List<ActorModel>();
            UpdateActorList = new List<ActorModel>();
        }

        public override void OnEnterMap()
        {
            ActorDict.Clear();
            ActorList.Clear();
            UpdateActorList.Clear();

            ActorRoot = MapCtrl.MapTrans.Find("Actors");
        }

        public override void OnExitMap()
        {
            ActorDict.Clear();
            ActorList.Clear();
            UpdateActorList.Clear();
        }

        public override void OnUpdate(float pDeltaTime, float pGameTime)
        {
            for (int i = 0; i < UpdateActorList.Count; i++)
            {
                if (UpdateActorList[i] != null && UpdateActorList[i].IsActive)
                {
                    UpdateActorList[i].UpdateLogic(pDeltaTime, pGameTime);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pActorUid">演员Uid</param>
        /// <param name="pActorId">演员Id</param>
        /// <param name="pActorGroupGoName">演员分组节点名</param>
        /// <returns></returns>
        public ActorModel CreateActor(string pActorUid, int pActorId, ActorType pActorType, string pActorGroupGoName = "")
        {
            if (!Config.ActorCfg.ContainsKey(pActorId))
            {
                Debug.LogError("创建演员失败，没有该配置:" + pActorId);
                return null;
            }

            ActorCfg actorCfg = Config.ActorCfg[pActorId];

            GameObject actorGo = GameEnv.Asset.CreateGo(actorCfg.prefab);
            if (!string.IsNullOrEmpty(pActorGroupGoName))
            {
                Transform actorRoot;
                if (!ActorRoot.Find(pActorGroupGoName, out actorRoot))
                {
                    actorRoot = new GameObject(pActorGroupGoName).transform;
                    actorRoot.transform.SetParent(ActorRoot);
                    actorRoot.transform.Reset();
                }
                actorGo.transform.SetParent(actorRoot);
            }
            else 
            {
                actorGo.transform.SetParent(ActorRoot);
            }

            ActorModel actor = null;
            switch (pActorType)
            {
                case ActorType.LocalPlayer:
                    actor = new Actor_LocalGamer(pActorUid, pActorId, pActorType, actorGo);
                    AddUpdateActor(actor);
                    break;
                case ActorType.RemotePlayer:
                    actor = new Actor_RemoteGamer(pActorUid, pActorId, pActorType, actorGo);
                    AddUpdateActor(actor);
                    break;
                case ActorType.Enemy:
                    actor = new Actor_AIGamer(pActorUid, pActorId, pActorType, actorGo);
                    AddUpdateActor(actor);
                    break;
            }

            if (!ActorDict.ContainsKey(pActorType))
            {
                ActorDict.Add(pActorType, new List<ActorModel>());
            }
            ActorDict[pActorType].Add(actor);
            ActorList.Add(actor);

            //数据存储后Init
            actor.Init();

            //发送事件
            ActorEvent.SendEvent(ActorEventDef.OnCreate, actor);

            return actor;
        }

        public ActorModel GetActor(string pActorUid)
        {
            for (int i = 0; i < ActorList.Count; i++)
            {
                if (ActorList[i].Uid == pActorUid)
                {
                    return ActorList[i];
                }
            }
            return null;
        }

        public List<T> GetActors<T>() where T : ActorModel 
        {
            List<T> resActors = new List<T>();
            for (int i = 0; i < ActorList.Count; i++)
            {
                if (ActorList[i] is T)
                {
                    resActors.Add((T)ActorList[i]);
                }
            }
            return resActors;
        }

        public void RemoveActor(ActorModel pActor)
        {
            List<ActorModel> actors = ActorDict[pActor.Type];
            if (actors.IsLegal())
                actors.Remove(pActor);

            ActorList.Remove(pActor);
            UpdateActorList.Remove(pActor);
            Object.Destroy(pActor.ActorGo);
            pActor.Clear();

            //发送事件
            ActorEvent.SendEvent(ActorEventDef.OnRemove, pActor);
        }

        public void AddUpdateActor(ActorModel pActor)
        {
            UpdateActorList.Add(pActor);
        }

        public void RemoveUpdateActor(ActorModel pActor)
        {
            UpdateActorList.Remove(pActor);
        }

        /// <summary>
        /// 设置玩家
        /// </summary>
        public void SetPlayer(ActorModel pActor)
        {
            if (pActor is Actor_LocalGamer == false)
            {
                Debug.LogError($"设置玩家失败，该类型不是玩家{pActor.GetType()}");
                return;
            }

            Player = pActor as Actor_LocalGamer;
        }

        public bool CheckClickActor(Vector3 pWorldPos, PointerEventData pEventData)
        {
            for (int i = 0; i < ActorList.Count; i++)
            {
                if (ActorList[i] != null)
                {
                    ActorModel checkActor = ActorList[i];
                    if (checkActor.ActorDisplay.ClickCollider.gameObject.activeInHierarchy && checkActor.ActorDisplay.ClickCollider.enabled == true)
                    {
                        BaseButton baseButton = checkActor.ActorDisplay.ClickCollider.GetComponent<BaseButton>();
                        if (baseButton != null && baseButton.HasClickEvent())
                        {
                            if (checkActor.ActorDisplay.ClickCollider.ContainPoint(pWorldPos))
                            {
                                baseButton.ExecuteClickFunc(pEventData);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
