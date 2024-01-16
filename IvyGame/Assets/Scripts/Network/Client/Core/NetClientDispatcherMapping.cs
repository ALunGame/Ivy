using ProtoBuf;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.Network.CDispatcher
{
    public class NetClientDispatcherMapping
    {
        private List<NetClientDispatcher> dispatchers = new List<NetClientDispatcher>();
        private Dictionary<ushort, Action<IExtensible>> msgDict = new Dictionary<ushort, Action<ProtoBuf.IExtensible>>();

        protected void AddDispatcher(NetClientDispatcher dispatcher)
        {
            dispatchers.Add(dispatcher);
        }

        public void AddDispatch<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            if (msgDict.ContainsKey(msgId))
            {
                Debug.LogError($"注册派发器失败，重复消息号:{msgId}");
                return;
            }

            msgDict.Add(msgId, (msgData) =>
            {
                T msg = (T)msgData;
                msgFunc(msg);
            });
        }

        public void OnReceiveMsg(ushort msgId, IExtensible msgData)
        {
            if(msgDict.ContainsKey(msgId))
            {
                msgDict[msgId]?.Invoke(msgData);
            }
        }
    }
}
