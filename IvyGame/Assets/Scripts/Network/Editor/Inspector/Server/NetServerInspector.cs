using UnityEditor;
using UnityEngine;

namespace Game.Network.Server
{
    [CustomEditor(typeof(NetServer))]
    internal class NetServerInspector : Editor
    {
        private NetServer netServer;

        private void OnEnable()
        {
            netServer = (NetServer)target;
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
