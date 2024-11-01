using System;
using UnityEngine;

namespace Game.Helper
{
    public abstract class Singleton<T>
    {
        protected static readonly T m_instance = Activator.CreateInstance<T>();
        public static T Instance { get { return m_instance; } }

        protected Singleton() { }
    }

    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T m_instance = default(T);

        private static GameObject GetGo(string path, Transform tranParent = null)
        {
            path = path.Replace("\\", "/");
            int index = path.IndexOf("/");
            string name = string.Empty;
            bool hasChild = false;
            if (index < 0)
                name = path;
            else
            {
                name = path.Substring(0, index);
                path = path.Substring(index + 1);
                hasChild = true;
            }

            Transform tran = null;
            if (tranParent == null)
            {
                if (string.IsNullOrEmpty(name))
                    return new GameObject("[Null-MonoSingleton]");
                GameObject root = GameObject.Find(name);
                if (root == null)
                    root = new GameObject(name);
                tran = root.transform;
            }
            else
            {
                tran = tranParent.Find(name);
                if (tran == null)
                {
                    tran = (new GameObject(name)).transform;
                    tran.SetParent(tranParent);
                }
            }

            return hasChild ? GetGo(path, tran) : tran.gameObject;
        }

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject go = GetGo(typeof(T).Name);
                    m_instance = go.GetComponent<T>();

                    if (m_instance == null)
                    {
                        m_instance = go.AddComponent<T>();
                    }
                    GameObject.DontDestroyOnLoad(go);
                    m_instance.OnInit();
                }
                return m_instance;
            }
        }

        public void CreateMono()
        {
        }

        protected virtual void OnInit()
        {

        }
    }
}
