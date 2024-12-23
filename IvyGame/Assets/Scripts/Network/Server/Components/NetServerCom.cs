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
            Logger.Client?.SetOpenLog(pOpen);
        }

        public bool GetOpenLogState()
        {
            bool? isOpen = Logger.Client?.GetOpenLog();
            return isOpen != null && (bool)isOpen;
        }

        public void OpenLogWarn(bool pOpen)
        {
            Logger.Client?.SetOpenLogWarning(pOpen);
        }

        public bool GetOpenLogWarnState()
        {
            bool? isOpen = Logger.Client?.GetOpenLogWarning();
            return isOpen != null && (bool)isOpen;
        }

        #endregion
    }
}
