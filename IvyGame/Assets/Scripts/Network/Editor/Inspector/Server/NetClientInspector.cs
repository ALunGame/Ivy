using Game.Network.Client;
using UnityEditor;
using UnityEngine;

namespace Game.Network.Server
{
    [CustomEditor(typeof(NetClient))]
    internal class NetClientInspector : Editor
    {
        private NetClient netClient;

        private void OnEnable()
        {
            netClient = (NetClient)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                bool isOpen = EditorGUILayout.Toggle("开启日志", netClient.GetOpenLogState());
                netClient.OpenLog(isOpen);

                isOpen = EditorGUILayout.Toggle("开启警告日志", netClient.GetOpenLogWarnState());
                netClient.OpenLogWarn(isOpen);
            }
        }
    }
}
