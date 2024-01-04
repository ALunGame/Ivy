
namespace Game.Network.CDispatcher
{
    public class CDispatcherMapping : NetDispatcherMapping
    {
        public CDispatcherMapping()
        {
            
            AddDispatcher(new CPlayerMsgDispatcher(this));

        }
    }
}

