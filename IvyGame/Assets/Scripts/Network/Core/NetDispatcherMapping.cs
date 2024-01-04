using ProtoBuf;
using System.Collections.Generic;
using System;

namespace Game.Network
{
    public class NetDispatcherMapping
    {
        private List<NetDispatcher> dispatchers = new List<NetDispatcher>();
        private Dictionary<ushort, Action<IExtensible>> msgDict = new Dictionary<ushort, Action<ProtoBuf.IExtensible>>();

        protected void AddDispatcher(NetDispatcher dispatcher)
        {
            dispatchers.Add(dispatcher);
        }

        public void AddDispatch<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            if (msgDict.ContainsKey(msgId))
            {
                NetworkLocate.Log.LogError("注册派发器失败，重复消息号", msgId);
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
