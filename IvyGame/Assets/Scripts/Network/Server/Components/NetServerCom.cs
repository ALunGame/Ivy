using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Game.Network.Server.Com
{
    /// <summary>
    /// 挂载服务器
    /// </summary>
    public class NetServerCom : MonoBehaviour
    {
        private NetServer netServer;
        private bool updateInThread = false;    

        private readonly object _onDrawGizmosLock = new object();
        public Action OnDrawGizmosFunc;

        public NetServer NetServer { get { return netServer; } }

        public void Init()
        {
            netServer = new NetServer();
            netServer.Init(updateInThread);

            NetServerLocate.Init(this);
        }

        public void StartServer()
        {
            netServer.Start();
        }

        public void EndServer()
        {
            netServer.Stop();
        }

        private void Update()
        {
            if (!updateInThread && netServer.IsActive)
            {
                netServer?.Update();
            }
        }

        private void OnDrawGizmosSelected()
        {
            lock (_onDrawGizmosLock)
            {
                OnDrawGizmosFunc?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            netServer?.Stop();
        }

        #region 编辑器

        public void OpenLog(bool pOpen)
        {
            if (Logger.Server == null)
                return;
            Logger.Server.OpenLog = pOpen;
        }

        public bool GetOpenLogState()
        {
            if (Logger.Server == null)
                return false;
            return Logger.Server.OpenLog;
        }

        public void OpenLogWarn(bool pOpen)
        {
            if (Logger.Server == null)
                return;
            Logger.Server.OpenLogWarning = pOpen;
        }

        public bool GetOpenLogWarnState()
        {
            if (Logger.Server == null)
                return false;
            return Logger.Server.OpenLogWarning;
        }

        #endregion
    }
}
