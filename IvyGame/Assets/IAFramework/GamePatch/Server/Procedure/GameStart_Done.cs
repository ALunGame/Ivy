using IAToolkit;
using IAUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_Done : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage(GamePatch.DefaultPackageName);
            YooAssets.SetDefaultPackage(gamePackage);

            //业务层初始化
            Debug.Log("游戏初始化成功！！！！！！！！！！！");
            GameContext.Init();
            GameContext.Asset.CreateGo("Test");

            UIPanelCfg cfg = IAConfig.Config.UIPanelCfg[IAUI.UIPanelDef.MainGamePanel];
            Debug.Log("UI"+cfg.script);

            //发送消息
            GamePatchData.Instance.SendProcessTips("游戏初始化成功 !");
            GamePatchData.Instance.SetGameStartState(EGamePatchState.Success);

            
        }
    }
}