using System;
using UnityEngine;

namespace Game.Network.Server.Com
{
    /// <summary>
    /// 挂载服务器
    /// </summary>
    public class NetServerCom : MonoBehaviour
    {
        private NetServer02 netServer;

        private readonly object _onDrawGizmosLock = new object();
        public Action OnDrawGizmosFunc;

        public NetServer02 NetServer { get { return netServer; } }

        public void Init()
        {
            netServer = new NetServer02();
            netServer.Init();
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
            NetServerLocate.Log.OpenLog = pOpen;
        }

        public bool GetOpenLogState()
        {
            return NetServerLocate.Log.OpenLog;
        }

        public void OpenLogWarn(bool pOpen)
        {
            NetServerLocate.Log.OpenWarn = pOpen;
        }

        public bool GetOpenLogWarnState()
        {
            return NetServerLocate.Log.OpenWarn;
        }


        #endregion
    }
}
