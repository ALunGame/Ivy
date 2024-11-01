using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Game.Network.Client
{
    internal class NetworkEvent
    {
        class MsgFuncBody
        {
            public string useName;
            public Action<IExtensible> msgFunc;
        }
        private static Dictionary<ushort, List<MsgFuncBody>> msgEventDict = new Dictionary<ushort, List<MsgFuncBody>>();

        public static void RegisterEvent(string pUseName, ushort pMsgId, Action<IExtensible> pMsgFunc)
        {
            if (!msgEventDict.ContainsKey(pMsgId))
            {
                msgEventDict.Add(pMsgId, new List<MsgFuncBody>());
            }

            List<MsgFuncBody> msgFuncBodies = msgEventDict[pMsgId];
            foreach (var item in msgFuncBodies)
            {
                if (item.useName == pUseName)
                {
                    NetClientLocate.Log.LogError($"注册网络事件失败，重复的使用者{pUseName}->{pMsgId}");
                    return;
                }
            }

            MsgFuncBody msgFuncBody = new MsgFuncBody();
            msgFuncBody.useName = pUseName;
            msgFuncBody.msgFunc = pMsgFunc;
            msgFuncBodies.Add(msgFuncBody);
        }

        public static void RemoveEvent(ushort pMsgId, Action<IExtensible> pMsgFunc)
        {
            if (!msgEventDict.ContainsKey(pMsgId))
            {
                return;
            }

            List<MsgFuncBody> msgFuncBodies = msgEventDict[pMsgId];
            for (int i = 0; i < msgFuncBodies.Count; i++)
            {
                if (msgFuncBodies[i].msgFunc == pMsgFunc)
                {
                    msgFuncBodies.RemoveAt(i);
                }
            }
        }

        public static void RemoveEvent(string pUseName)
        {
            foreach (var msgFuncBodies in msgEventDict.Values)
            {
                for (int i = 0; i < msgFuncBodies.Count; i++)
                {
                    if (msgFuncBodies[i].useName == pUseName)
                    {
                        msgFuncBodies.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 广播事件
        /// </summary>
        public static void BroadcastEvent(ushort pMsgId, IExtensible pMsgBody)
        {
            if (!msgEventDict.ContainsKey(pMsgId))
            {
                return;
            }
            List<MsgFuncBody> msgFuncBodies = msgEventDict[pMsgId];
            for (int i = 0; i < msgFuncBodies.Count; i++)
            {
                msgFuncBodies[i].msgFunc?.Invoke(pMsgBody);
            }
        }

        public static void Clear()
        {
            msgEventDict.Clear();
        }
    }
}
