using IAEngine;
using IAFramework.Server;
using IAFramework.UI;
using IAUI;
using System.Collections;
using UnityEngine;
using YooAsset;

namespace IAFramework
{
    public class GamePatch : MonoBehaviour
    {
        public const string DefaultPackageName = "DefaultPackage";

        [SerializeField]
        private EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        [SerializeField]
        private Transform PatchPanelTrans;
        private PatchPanel PatchPanel = new PatchPanel();

        private GameStartServer startServer = new GameStartServer();

        private void Awake()
        {
            Debug.Log($"资源系统运行模式：{PlayMode}");
            Application.targetFrameRate = 60;
            Application.runInBackground = true;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);

            //创建界面
            CreatePatchPanel();

            //开始默认补丁更新流程
            startServer.SetData(DefaultPackageName, PlayMode);
            startServer.Init();
            startServer.Start();
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
