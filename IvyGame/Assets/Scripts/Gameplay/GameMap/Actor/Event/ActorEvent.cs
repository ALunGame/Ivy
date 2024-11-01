using System;
using System.Collections.Generic;

namespace Gameplay.GameMap.Actor
{
    public enum ActorEventDef
    {
        /// <summary>
        /// 当演员被创建
        /// </summary>
        OnCreate,

        /// <summary>
        /// 当演员被移除,节点被销毁
        /// </summary>
        OnRemove,

        /// <summary>
        /// 当玩家摸牌
        /// </summary>
        OnDrawCard,

        /// <summary>
        /// 当玩家出牌
        /// </summary>
        OnPlayCard,

        /// <summary>
        /// 当拿去别人的牌
        /// </summary>
        OnTokenOtherCard,
    }

    public static class ActorEvent
    {
        private static Dictionary<ActorEventDef, Action<ActorModel>> eventDict = new Dictionary<ActorEventDef, Action<ActorModel>>();

        public static void Init()
        {

        }

        public static void Clear()
        {
            eventDict.Clear();
        }

        public static void SendEvent(ActorEventDef pDef, ActorModel pActor)
        {
            if (!eventDict.ContainsKey(pDef))
                return;

            Delegate[] delegates = eventDict[pDef].GetInvocationList();
            foreach (Delegate d in delegates)
            {
                d.DynamicInvoke(pActor);
            }
        }

        public static void RegEvent(ActorEventDef pDef, Action<ActorModel> pAction)
        {
            if (!eventDict.ContainsKey(pDef))
            {
                eventDict.Add(pDef, pAction);
            }
            else
            {
                eventDict[pDef] += pAction;
            }
        }

        public static void RemoveEvent(ActorEventDef pDef, Action<ActorModel> pAction)
        {
            if (!eventDict.ContainsKey(pDef))
                return;
            eventDict[pDef] -= pAction;
        }
    }
}
