using ProtoBuf;
using System;

namespace Game.Network.CDispatcher
{
    public class NetClientDispatcher
    {
        private NetClientDispatcherMapping mapping;

        public NetClientDispatcher(NetClientDispatcherMapping InMapping)
        {
            mapping = InMapping;
        }

        protected void AddDispatch<T>(ushort msgId, Action<T> msgFunc) where T : IExtensible
        {
            mapping.AddDispatch(msgId, msgFunc);
        }
    }
}
