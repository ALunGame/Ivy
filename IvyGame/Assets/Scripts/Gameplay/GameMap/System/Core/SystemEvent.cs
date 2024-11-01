using System;
using System.Collections.Generic;

namespace Gameplay.GameMap.System
{
    public enum SystemEventDef
    {
        /// <summary>
        /// 当创建关卡时
        /// </summary>
        OnCreateLevel,

        /// <summary>
        /// 当开始关卡时
        /// </summary>
        OnStartLevel,

        /// <summary>
        /// 当暂停关卡时
        /// </summary>
        OnPauseLevel,

        /// <summary>
        /// 当恢复关卡时
        /// </summary>
        OnResumeLevel,

        /// <summary>
        /// 当结束关卡时
        /// </summary>
        OnEndLevel,
    }


    public static class SystemEvent
    {
        private static Dictionary<SystemEventDef, Action<BaseGameMapSystem>> eventDict = new Dictionary<SystemEventDef, Action<BaseGameMapSystem>>();

        public static void Init()
        {

        }

        public static void Clear()
        {
            eventDict.Clear();
        }

        public static void SendEvent(SystemEventDef pDef, BaseGameMapSystem pSystem)
        {
            if (!eventDict.ContainsKey(pDef))
                return;

            Delegate[] delegates = eventDict[pDef].GetInvocationList();
            foreach (Delegate d in delegates)
            {
                d.DynamicInvoke(pSystem);
            }
        }

        public static void RegEvent(SystemEventDef pDef, Action<BaseGameMapSystem> pAction)
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

        public static void RemoveEvent(SystemEventDef pDef, Action<BaseGameMapSystem> pAction)
        {
            if (!eventDict.ContainsKey(pDef))
                return;
            eventDict[pDef] -= pAction;
        }
    }
}
