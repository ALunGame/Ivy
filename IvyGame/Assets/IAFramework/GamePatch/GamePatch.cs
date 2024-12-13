using Cysharp.Threading.Tasks;
using IAFramework.Patch;
using IAFramework.UI;
using IAUI;
using UnityEngine;
using YooAsset;

namespace IAFramework
{
    internal class GamePatch : MonoBehaviour
    {
        [Header("PathUI节点")]
        [SerializeField]
        private Transform pathTrans;
        private PatchPanel patchPanel;

        public async UniTask Init()
        {
            //初始化资源系统
            YooAssets.Initialize();

            //创建界面
            CreatePatchPanel();

            // 开始补丁更新流程
            for (int i = 0; i < GameGlobal.Instance.FixedConfig.LoadPackages.Count; i++)
            {
                PackageLoadData tData = GameGlobal.Instance.FixedConfig.LoadPackages[i];
                PatchOperation operation = new PatchOperation(tData.PackageName, tData.PackageBuildPipeline.ToString(), GameGlobal.Instance.AssetPlayMode);
                YooAssets.StartOperation(operation);
                await operation;
            }

            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage(GameGlobal.DefaultPackageName);
            YooAssets.SetDefaultPackage(gamePackage);

            //业务层初始化
            Debug.Log("游戏初始化成功！！！！！！！！！！！");

            //发送消息
            GamePatchData.Instance.SendProcessTips("游戏初始化成功 !");
            GamePatchData.Instance.SetGamePatchState(EGamePatchState.Success);
        }

        public void Clear()
        {
            
        }
        
        private void CreatePatchPanel()
        {
            patchPanel = new PatchPanel();  
            //加载更新页面
            UIPanelCreater.CreateUIPanelTrans(patchPanel, pathTrans);
            patchPanel.Show();
        }
    }
}
