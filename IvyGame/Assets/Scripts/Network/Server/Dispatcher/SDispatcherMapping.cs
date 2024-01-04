
namespace Game.Network.SDispatcher
{
    public class SDispatcherMapping : NetDispatcherMapping
    {
        public SDispatcherMapping()
        {
            
            AddDispatcher(new SPlayerMsgDispatcher(this));

        }
    }
}

