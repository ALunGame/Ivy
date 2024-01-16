using LiteNetLib;
using ProtoBuf;
using System;

namespace Game.Network.SDispatcher
{
    internal class NetServerDispatcher
    {
        private NetServerDispatcherMapping mapping;

        internal NetServerDispatcher(NetServerDispatcherMapping InMapping)
        {
            mapping = InMapping;
        }

        protected void AddDispatch<T>(ushort msgId, Action<NetPeer, T> msgFunc) where T : IExtensible
        {
            mapping.AddDispatch(msgId, msgFunc);
        }
    }
}
