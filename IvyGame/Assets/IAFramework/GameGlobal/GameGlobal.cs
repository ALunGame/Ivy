using Cysharp.Threading.Tasks;
using Game.Helper;
using GameContext;
using IAEngine;
using IAUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace IAFramework
{
    /// <summary>
    /// 游戏资源包配置
    /// </summary>
    [Serializable]
    public class PackageLoadData
    {
        public string PackageName;
        public EDefaultBuildPipeline PackageBuildPipeline;
    }

    /// <summary>
    /// 游戏全局固定配置
    /// </summary>
    [Serializable]
    public class GameGlobalFixedConfig
    {
        [Header("需要初始化的游戏资源包")]
        public List<PackageLoadData> LoadPackages = new List<PackageLoadData>();

        [Header("音乐游戏资源包名")]
        public string AudioPackageName = "";

        [Header("原生游戏资源包名")]
        public string RawFilePackageName = "";
    }

    /// <summary>
    /// 游戏全局动态配置
    /// </summary>
    [Serializable]
    public class GameGlobalDynamicConfig
    {

    }


    /// <summary>
    /// 游戏全局配置
    /// 1，挂载在开始场景，全局唯一
    /// 2，配置游戏初始化参数
    /// </summary>
    public class GameGlobal : MonoSingleton<GameGlobal>
    {
        public const string DefaultPackageName = "DefaultPackage";

        [Header("游戏全局固定配置")]
        [SerializeField]
        private GameGlobalFixedConfig fixedConfig = new GameGlobalFixedConfig();

        /// <summary>
        /// 游戏全局固定配置
        /// </summary>
        public GameGlobalFixedConfig FixedConfig { get => fixedConfig; }

        [Header("游戏全局动态配置")]
        [SerializeField]
        private GameGlobalDynamicConfig dynamicConfig = new GameGlobalDynamicConfig();

        /// <summary>
        /// 游戏全局动态配置
        /// </summary>
        public GameGlobalDynamicConfig DynamicConfig { get => dynamicConfig; }


        [Header("游戏资源运行模式")]
        public EPlayMode AssetPlayMode = EPlayMode.EditorSimulateMode;

        [Header("GamePatch")]
        [SerializeField]
        private GamePatch gamePatch;

        [Header("UI中心")]
        [SerializeField]
        private UICenter uICenter;

        protected override void OnInit()
        {
            Debug.Log($"资源系统运行模式：{AssetPlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }

        private void Start()
        {
            StartGame().Forget();
        }

        private void OnDestroy()
        {
            DestroyGame();
        }

        private async UniTaskVoid StartGame()
        {
            GameEnv.Init();

            //游戏流程初始化
            await gamePatch.Init();

            //设置音乐资源包
            GameEnv.Asset.SetRawFilePackage(FixedConfig.AudioPackageName);

            //设置原生资源包
            GameEnv.Asset.SetRawFilePackage(FixedConfig.RawFilePackageName);

            //缓存池
            CachePool.Init();

            //UI中心初始化
            await uICenter.Init();

            //游戏存档初始化
            GameContextLocate.Init();

            //显示起始UI
            UILocate.UI.Show(UIPanelDef.MainGamePanel);
            await UniTask.Yield(PlayerLoopTiming.Update);

            //设置状态
            GamePatchData.Instance.SetGamePatchState(EGamePatchState.GameStartSuccess);
        }

        private void DestroyGame()
        {
            GameEnv.Clear();
            CachePool.Clear();
            gamePatch.Clear();
        }

        public void AddChild(GameObject pGo)
        {
            if (pGo.IsNull())
            {
               return;
            }

            pGo.transform.SetParent(transform, false);
        }
    } 
}
