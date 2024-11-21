using IAServer;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace IAFramework.Server
{
    public class AssetServer : BaseServer
    {
        #region 场景加载

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="physicsMode">场景物理模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public SceneHandle LoadSceneAsync(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false, uint priority = 100)
        {
            return YooAssets.LoadSceneAsync(location, sceneMode, physicsMode, suspendLoad, priority);
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
            AssetHandle handle = YooAssets.LoadAssetSync<TObject>(location);
            return handle.AssetObject as TObject;
        }
        
        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public AssetHandle LoadAssetAsync<TObject>(string location) where TObject : UnityEngine.Object
        {
            AssetHandle handle = YooAssets.LoadAssetAsync<TObject>(location);
            return handle;
        }

        #endregion

        #region 原生文件

        private string rawFilePackageName;
        private ResourcePackage rawFilePackage;
        public ResourcePackage RawFilePackage 
        {
            get
            {
                if (rawFilePackage == null)
                {
                    rawFilePackage = YooAssets.GetPackage(rawFilePackageName);
                }
                return rawFilePackage;
            }
        }

        /// <summary>
        /// 设置原生资源Package包
        /// </summary>
        /// <param name="pPackageName"></param>
        public void SetRawFilePackage(string pPackageName)
        {
            rawFilePackageName = pPackageName;
            rawFilePackage = YooAssets.GetPackage(pPackageName);
            Debug.LogError($"SetRawFilePackage::{rawFilePackage.PackageName}");
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public RawFileHandle LoadRawFileSync(string location)
        {
            RawFileHandle handle = RawFilePackage.LoadRawFileSync(location);
            return handle;
        }
        
        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public RawFileHandle LoadRawFileAsync(string location)
        {
            RawFileHandle handle = RawFilePackage.LoadRawFileAsync(location);
            return handle;
        }

        #endregion

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
            RawFileHandle handle = LoadRawFileSync(assetName);
            return handle.GetRawFileData();
        }
        
        /// <summary>
        /// 加载字符串
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string LoadString(string assetName)
        {
            RawFileHandle handle = LoadRawFileSync(assetName);
            return handle.GetRawFileText();
        }

        #endregion
    }
}