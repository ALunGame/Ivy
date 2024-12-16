using IAServer;

namespace Game.Network.Server
{
    internal class NetServerLog : LogServer
    {
        private static readonly object DebugLogLock = new object();
        //public override string LogTag => "<color=#FBBC00>Server</color>";
        public override string LogTag => "Server";

        public NetServerLog() 
        {
            OpenLog = false;
        }

        public override void Log(string log, params object[] args)
        {
            lock (DebugLogLock)
            {
                base.Log(log, args);
            }
        }

        public override void LogError(string log, params object[] args)
        {
            lock (DebugLogLock)
            {
                base.LogError(log, args); 
            }
        }

        public override void LogR(string log, params object[] args)
        {
            lock (DebugLogLock)
            {
                base.LogR(log, args); 
            }
        }

        public override void LogWarning(string log, params object[] args)
        {
            lock (DebugLogLock)
            {
                base.LogWarning(log, args); 
            }
        }
    }
}
