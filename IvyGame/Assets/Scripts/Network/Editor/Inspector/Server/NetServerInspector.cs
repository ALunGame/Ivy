using Game.Network.Server.Com;
using UnityEditor;
using UnityEngine;

namespace Game.Network.Server
{
    [CustomEditor(typeof(NetServerCom))]
    internal class NetServerInspector : Editor
    {
        private NetServerCom netServer;

        private void OnEnable()
        {
            netServer = (NetServerCom)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                bool isOpen = EditorGUILayout.Toggle("开启日志", netServer.GetOpenLogState());
                netServer.OpenLog(isOpen);

                isOpen = EditorGUILayout.Toggle("开启警告日志", netServer.GetOpenLogWarnState());
                netServer.OpenLogWarn(isOpen);
            }
        }
    }
}
