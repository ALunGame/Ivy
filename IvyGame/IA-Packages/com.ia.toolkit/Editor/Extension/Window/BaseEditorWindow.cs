using System.Collections;
using UnityEditor;
using UnityEngine;
using IAToolkit.Core;

namespace IAToolkit
{
    public abstract class BaseEditorWindow : EditorWindow
    {
        static GUIStyle csIconStyle;
        static GUIContent csIcon;

        public static GUIStyle CSIconStyle
        {
            get
            {
                if (csIconStyle == null)
                {
                    csIconStyle = new GUIStyle(GUI.skin.button);
                    csIconStyle.padding.left = 0;
                    csIconStyle.padding.right = 0;
                    csIconStyle.padding.top = 0;
                    csIconStyle.padding.bottom = 0;
                }
                return csIconStyle;
            }
        }
        public static GUIContent CSIcon
        {
            get
            {
                if (csIcon == null)
                    csIcon = new GUIContent(EditorGUIUtility.FindTexture("cs Script Icon"), "打开窗口脚本");
                return csIcon;
            }
        }

        MonoScript monoScript;
        MonoScript MonoScript
        {
            get
            {
                if (monoScript == null)
                    monoScript = MonoScript.FromScriptableObject(this);
                return monoScript;
            }
        }

        EditorCoroutineService coroutineMachine;
        EditorCoroutineService CoroutineMachine
        {
            get
            {
                if (coroutineMachine == null)
                    coroutineMachine = new EditorCoroutineService();
                return coroutineMachine;
            }
        }

        protected virtual void ShowButton(Rect rect)
        {
            rect.x -= 8;
            rect.width = 20;
            if (GUI.Button(rect, CSIcon, CSIconStyle))
                AssetDatabase.OpenAsset(MonoScript);
        }

        protected virtual void Update()
        {
            CoroutineMachine.Update();
        }

        public EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            return CoroutineMachine.StartCoroutine(enumerator);
        }

        public void StopCoroutine(EditorCoroutine coroutine)
        {
            CoroutineMachine.StopCoroutine(coroutine);
        }
    }
}
