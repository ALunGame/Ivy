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
        public EPlayMode PlayMode { private set; get; }
        public string PackageName { private set; get; }
        
        //游戏开始流程
        private Fsm<GameStartServer> procedure;

        public void SetData(string packageName, EPlayMode playMode)
        {
            PackageName = packageName;
            PlayMode = playMode;
        }

        public override void OnInit()
        {
            procedure = Fsm<GameStartServer>.Create(this,new List<FsmState<GameStartServer>>()
            {
                new GameStart_Prepare(),
                new GameStart_Initialize(),
                new GameStart_UpdatePackageVersion(),
                new GameStart_UpdatePackageManifest(),
                new GameStart_CreatePackageDownloader(),
                new GameStart_DownloadPackageFiles(),
                new GameStart_DownloadPackageOver(),
                new GameStart_ClearPackageCache(),
                new GameStart_Done(),
            });
        }

        public override void OnClear()
        {
            procedure?.Clear();
        }

        public void Start()
        {
            procedure?.Start(typeof(GameStart_Prepare));
        }
    }
}