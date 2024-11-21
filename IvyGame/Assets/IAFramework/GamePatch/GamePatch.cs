using Cysharp.Threading.Tasks;
using GameContext;
using IAEngine;
using IAFramework.Patch;
using IAFramework.UI;
using IAUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace IAFramework
{
    internal class GamePatch : MonoBehaviour
    {
        public const string DefaultPackageName = "DefaultPackage";

        [Serializable]
        public class PackageLoadData
        {
            public string PackageName;
            public EDefaultBuildPipeline PackageBuildPipeline;
        }

        [SerializeField]
        private EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        [SerializeField]
        private Transform PatchPanelTrans;
        private PatchPanel PatchPanel = new PatchPanel();

        [Header("需要加载包")]
        [SerializeField]
        private List<PackageLoadData> LoadPackages = new List<PackageLoadData>();
        //原生文件包名
        [Header("原生文件包名")]
        [SerializeField]
        private string RawFilePackageName = "";

        private void Awake()
        {
            Debug.Log($"资源系统运行模式：{PlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            DontDestroyOnLoad(this.gameObject);
        }

        IEnumerator Start()
        {
            //初始化资源系统
            YooAssets.Initialize();

            //创建界面
            CreatePatchPanel();

            // 开始补丁更新流程
            for (int i = 0; i < LoadPackages.Count; i++)
            {
                PackageLoadData tData = LoadPackages[i];
                PatchOperation operation = new PatchOperation(tData.PackageName, tData.PackageBuildPipeline.ToString(), PlayMode);
                YooAssets.StartOperation(operation);
                yield return operation;
            }

            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage(DefaultPackageName);
            YooAssets.SetDefaultPackage(gamePackage);

            //业务层初始化
            Debug.Log("游戏初始化成功！！！！！！！！！！！");
            GameEnv.Init();

            //设置原生资源包
            GameEnv.Asset.SetRawFilePackage(RawFilePackageName);

            //发送消息
            GamePatchData.Instance.SendProcessTips("游戏初始化成功 !");
            GamePatchData.Instance.SetGamePatchState(EGamePatchState.Success);

            //StartGame().Forget();
        }


        public async UniTaskVoid StartGame()
        {
            CachePool.Init();

            //游戏存档初始化
            GameContextLocate.Init();

            //UI初始化
            UICenter uICenter = GameObject.Find("Game/UICenter").GetComponent<UICenter>();
            uICenter.Init();
            await UniTask.Yield(PlayerLoopTiming.Update);

            //显示起始UI
            UILocate.UI.Show(UIPanelDef.MainGamePanel);
            await UniTask.Yield(PlayerLoopTiming.Update);

            GamePatchData.Instance.SetGamePatchState(EGamePatchState.GameStartSuccess);
        }

        private void OnApplicationQuit()
        {
            GameEnv.Clear();
        }
        
        private void CreatePatchPanel()
        {
            // 加载更新页面
            UIPanelCreater.CreateUIPanelTrans(PatchPanel, PatchPanelTrans);
            PatchPanel.Show();
        }
    }
}
