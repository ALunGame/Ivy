using IAServer;

namespace Game.Network.Client
{
    internal class NetClientLog : LogServer
    {
        public override string LogTag => "<color=#008EFF>Client</color>";

        public NetClientLog() 
        {
            OpenLog = false;
        }
    }
}
