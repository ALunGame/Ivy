using ProtoBuf;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.Network.CDispatcher
{
    public delegate void ListenMsgFunc<T>(T msgData);

    public class NetClientDispatcherMapping
    {
        private List<NetClientDispatcher> dispatchers = new List<NetClientDispatcher>();
        private Dictionary<ushort, Action<IExtensible>> msgDict = new Dictionary<ushort, Action<ProtoBuf.IExtensible>>();

        private Dictionary<ushort, Dictionary<int,Action<IExtensible>>> listenDict = new Dictionary<ushort, Dictionary<int, Action<IExtensible>>>();


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
                if (listenDict.ContainsKey(msgId))
                {
                    foreach (var item in listenDict[msgId].Values)
                    {
                        item?.Invoke(msgData);
                    }
                }
            }
        }

        public void AddListen<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            int funcKey = msgFunc.GetHashCode();
            if (!listenDict.ContainsKey(msgId))
                listenDict.Add(msgId, new Dictionary<int, Action<IExtensible>>());   
            if (listenDict[msgId].ContainsKey(funcKey))
                listenDict[msgId].Remove(funcKey);

            listenDict[msgId].Add(funcKey, (msgData) =>
            {
                T msg = (T)msgData;
                msgFunc(msg);
            });
        }

        public void RemoveListen<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            int funcKey = msgFunc.GetHashCode();
            if (!listenDict.ContainsKey(msgId))
                return;
            if (!listenDict[msgId].ContainsKey(funcKey))
                return;

            listenDict[msgId].Remove(funcKey);
        }
    }
}
