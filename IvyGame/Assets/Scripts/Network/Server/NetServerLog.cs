using IAServer;

namespace Game.Network.Server
{
    internal class NetServerLog : LogServer
    {
        public override string LogTag => "<color=#FBBC00>Server</color>";

        public NetServerLog() 
        {
            OpenLog = true;
        }
    }
}
