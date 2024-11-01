
namespace Game.Network.CDispatcher
{
    internal class CDispatcherMapping : NetClientDispatcherMapping
    {
        internal CDispatcherMapping()
        {
            AddDispatcher(new CRoomMsgDispatcher(this));

        }
    }
}

