using ProtoBuf;
using System;

namespace Game.Network
{
    public class NetDispatcher
    {
        private NetDispatcherMapping mapping;

        public NetDispatcher(NetDispatcherMapping InMapping)
        {
            mapping = InMapping;
        }

        protected void AddDispatch<T>(ushort msgId,Action<T> msgFunc) where T : IExtensible
        {
            mapping.AddDispatch(msgId, msgFunc);
        }
    }
}
