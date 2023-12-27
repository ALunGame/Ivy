using IAServer;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace IAFramework.Server
{
    public class AssetServer : BaseServer
    {
        private ResourcePackage _package;
        
        public override void OnInit()
        {
            _package = YooAsset.YooAssets.GetPackage("DefaultPackage");
        }

        public override void OnClear()
        {
            _package = null;
        }

        #region 场景加载

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public SceneOperationHandle LoadSceneAsync(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            return _package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public TObject LoadAssetSync<TObject>(string location) where TObject : UnityEngine.Object
        {
            AssetOperationHandle handle = _package.LoadAssetSync<TObject>(location);
            return handle.AssetObject as TObject;
        }
        
        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public AssetOperationHandle LoadAssetAsync<TObject>(string location) where TObject : UnityEngine.Object
        {
            AssetOperationHandle handle = _package.LoadAssetAsync<TObject>(location);
            return handle;
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public RawFileOperationHandle LoadRawFileSync(string location)
        {
            RawFileOperationHandle handle = _package.LoadRawFileSync(location);
            return handle;
        }
        
        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public RawFileOperationHandle LoadRawFileAsync(string location)
        {
            RawFileOperationHandle handle = _package.LoadRawFileAsync(location);
            return handle;
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void UnloadAssets()
        {
            _package?.UnloadUnusedAssets();
        }

        #region 通用扩展

        /// <summary>
        /// 创建预制体
        /// </summary>
        /// <param name="prefabName">预制体名</param>
        /// <returns></returns>
        public GameObject CreateGo(string prefabName)
        {
            GameObject goAsset = LoadPrefab(prefabName);
            if (goAsset == null)
            {
                return null;
            }

            return GameObject.Instantiate(goAsset);
        }
        
        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="prefabName">预制体名</param>
        /// <returns></returns>
        public GameObject LoadPrefab(string prefabName)
        {
            return LoadAssetSync<GameObject>(prefabName);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string spriteName)
        {
            return LoadAssetSync<Sprite>(spriteName);
        }

        /// <summary>
        /// 加载源文件字节流
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public byte[] LoadBytes(string assetName)
        {
            RawFileOperationHandle handle = LoadRawFileSync(assetName);
            return handle.GetRawFileData();
        }
        
        /// <summary>
        /// 加载字符串
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string LoadString(string assetName)
        {
            TextAsset textAsset = LoadAssetSync<TextAsset>(assetName);
            if (textAsset == null)
                return "";
            return textAsset.text;
        }

        #endregion
    }
}