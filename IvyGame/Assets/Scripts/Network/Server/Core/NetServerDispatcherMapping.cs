using Game.Network.Server;
using LiteNetLib;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Game.Network.SDispatcher
{
    internal class NetServerDispatcherMapping
    {
        private List<NetServerDispatcher> dispatchers = new List<NetServerDispatcher>();
        private Dictionary<ushort, Action<NetPeer, IExtensible>> msgDict = new Dictionary<ushort, Action<NetPeer, IExtensible>>();

        protected void AddDispatcher(NetServerDispatcher dispatcher)
        {
            dispatchers.Add(dispatcher);
        }

        public void AddDispatch<T>(ushort msgId, Action<NetPeer, T> msgFunc) where T : IExtensible
        {
            if (msgDict.ContainsKey(msgId))
            {
                NetServerLocate.Log.LogError($"注册派发器失败，重复消息号:{msgId}");
                return;
            }

            msgDict.Add(msgId, (peer, msgData) =>
            {
                T msg = (T)msgData;
                msgFunc(peer, msg);
            });
        }

        public void OnReceiveMsg(NetPeer peer, ushort msgId, IExtensible msgData)
        {
            if (msgDict.ContainsKey(msgId))
            {
                msgDict[msgId]?.Invoke(peer, msgData);
            }
        }
    }
}
