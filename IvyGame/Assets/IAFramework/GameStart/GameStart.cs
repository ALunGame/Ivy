using IAEngine;
using IAFramework.Server;
using IAFramework.UI;
using IAUI;
using System.Collections;
using UnityEngine;
using YooAsset;

namespace IAFramework
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField]
        private EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        [SerializeField]
        private Transform PatchPanelTrans;

        public const string DefaultPackageName = "DefaultPackage";

        private PatchPanel PatchPanel = new PatchPanel();
        
        private void Awake()
        {
            Debug.Log($"资源系统运行模式：{PlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            DontDestroyOnLoad(this.gameObject);
        }

        private IEnumerator Start()
        {
            //初始化资源系统
            YooAssets.Initialize();

            //创建界面
            CreatePatchPanel();

            // 开始补丁更新流程
            PatchOperation operation = new PatchOperation(DefaultPackageName, EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), PlayMode);
            YooAssets.StartOperation(operation);
            yield return operation;

            // 设置默认的资源包
            var gamePackage = YooAssets.GetPackage(DefaultPackageName);
            YooAssets.SetDefaultPackage(gamePackage);

            Debug.Log("游戏初始化成功！！！！！！！！！！！");
            GameContext.Init();
        }
        
        private void OnApplicationQuit()
        {
            YooAssets.Destroy();
            GameContext.Clear();
        }
        
        private void CreatePatchPanel()
        {
            // 加载更新页面
            UIPanelCreater.CreateUIPanelTrans(PatchPanel, PatchPanelTrans);
            PatchPanel.Show();
        }
    }
}
