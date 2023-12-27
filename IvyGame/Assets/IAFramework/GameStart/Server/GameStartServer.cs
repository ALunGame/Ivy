using System;
using System.Collections.Generic;
using IAFramework.Server.Procedure;
using IAServer;
using IAToolkit;
using YooAsset;

namespace IAFramework.Server
{
    public class GameStartServer : BaseServer
    {
        /// <summary>
        /// 资源模式
        /// </summary>
        public EPlayMode AssetMode { private set; get; }
        
        //游戏开始流程
        private Fsm<GameStartServer> procedure;

        //开始成功
        public Action OnStartSuccess;
        //开始失败
        public Action<string> OnStartFail;

        public override void OnInit()
        {
            procedure = Fsm<GameStartServer>.Create(this,new List<FsmState<GameStartServer>>()
            {
                new GameStart_Prepare(),
                new GameStart_Initialize(),
                new GameStart_UpdateVersion(),
                new GameStart_UpdateManifest(),
                new GameStart_CreateDownloader(),
                new GameStart_DownloadFiles(),
                new GameStart_ClearAssetCache(),
                new GameStart_Done(),
            });
        }

        public override void OnClear()
        {
            OnStartSuccess = null;
            OnStartFail = null;
            procedure.Clear();
        }

        public void Start(EPlayMode pAssetMode)
        {
            AssetMode = pAssetMode;
            procedure.Start(typeof(GameStart_Prepare));
        }
    }
}