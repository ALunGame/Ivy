using Game.Network.Server;
using LiteNetLib;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Game.Network.SDispatcher
{
    /// <summary>
    /// 服务端消息派发
    /// 1，更简单，没有消息监听，因为目前觉得服务端做的事情应该要更纯粹，数据流向更为单一
    /// </summary>
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
                Logger.Server?.LogError($"注册派发器失败，重复消息号:{msgId}");
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
