using IAEngine;
using System.Collections.Generic;
using UnityEngine;

namespace IAUI
{
    /// <summary>
    /// 缓存对象
    /// </summary>
    public class UICacheGlue : UIGlue
    {
        //激活池
        private List<GameObject> activePool = new List<GameObject>();

        //缓存池
        private List<GameObject> cachePool = new List<GameObject>();

        //缓存存放根节点路径
        private string cacheRootPath;
        //缓存存放根节点
        private Transform cacheRoot;

        //是否是界面中节点
        private bool isLocal;
        //缓存对象名
        private string cacheItemName;
        //缓存对象
        private GameObject cacheItem;

        //需不需要隐藏回收
        private bool needHideRecycle = false;

        /// <summary>
        /// 缓存对象
        /// </summary>
        /// <param name="cacheItemPath">缓存对象路径或者名</param>
        /// <param name="cacheRootPath">缓存对象存放根节点路径</param>
        /// <param name="isLocal">是否是界面中节点</param>
        /// <param name="needHideRecycle">需不需要隐藏回收</param>
        public UICacheGlue(string cacheItemPath, string cacheRootPath, bool isLocal = false, bool needHideRecycle = true)
        {
            this.cacheItemName = cacheItemPath;
            this.cacheRootPath = cacheRootPath;
            this.isLocal = isLocal; 
            this.needHideRecycle = needHideRecycle;
        }

        public override void OnBeforeAwake(InternalUIPanel panel)
        {
            base.OnBeforeAwake(panel);
            if (string.IsNullOrEmpty(cacheRootPath))
            {
                cacheRoot = panel.transform;
                return;
            }
            cacheRoot = panel.transform.Find(cacheRootPath);
            if (cacheRoot == null)
            {
                UILocate.Log.LogError("缓存对象绑定失败，没有缓存存放路径！！", cacheRootPath);
            }
        }

        public override void OnHide(InternalUIPanel panel)
        {
            if (needHideRecycle)
            {
                RecycleAll();
            }
        }

        public override void OnDestroy(InternalUIPanel panel)
        {
            cacheItem = null;
            cacheRoot = null;
            cachePool.Clear();
            activePool.Clear();
        }

        public GameObject Take()
        {
            GameObject cacheItem;
            if (cachePool.Count > 0)
            {
                cacheItem = cachePool[0];
                cachePool.RemoveAt(0);
            }
            else
            {
                cacheItem = CreateCache();
            }
            cacheItem.SetActive(true);
            activePool.Add(cacheItem);
            return cacheItem;
        }

        public void RecycleAll()
        {
            for (int i = 0; i < activePool.Count; i++)
            {
                GameObject cacheItem = activePool[i];
                cacheItem.SetActive(false);
                cachePool.Add(cacheItem);
            }
            activePool.Clear();
        }

        private GameObject CreateCache()
        {
            if (cacheItem == null)
            {
                if (_Panel.transform == null)
                {
                    UILocate.Log.LogError("缓存对象绑定失败，界面被销毁！！");
                    return null;
                }
                else
                {
                    if (isLocal)
                    {
                        cacheItem = _Panel.transform.Find(cacheItemName).gameObject;
                        if (cacheItem == null)
                            UILocate.Log.LogError("缓存对象绑定失败，界面中没有缓存对象！！", cacheItemName, _Panel.transform.name);
                    }
                    else
                    {
                        cacheItem = IAFramework.GameEnv.Asset.LoadPrefab(cacheItemName);
                        if (cacheItem == null)
                            UILocate.Log.LogError("缓存对象绑定失败，没有缓存对象！！", cacheItemName);
                    }
                }
            }

            if (cacheItem == null)
                return null;
            
            GameObject item = GameObject.Instantiate(cacheItem);
            item.transform.SetParent(cacheRoot);
            item.transform.Reset();
            return item;
        }
    }
}