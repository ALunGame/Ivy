
namespace Game.Network.SDispatcher
{
    internal class SDispatcherMapping : NetServerDispatcherMapping
    {
        internal SDispatcherMapping()
        {
            
            AddDispatcher(new SRoomMsgDispatcher(this));

        }
    }
}

