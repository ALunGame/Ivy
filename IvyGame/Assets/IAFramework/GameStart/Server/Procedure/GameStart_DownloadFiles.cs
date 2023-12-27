using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_DownloadFiles_Context : FsmStateContext
    {
        public ResourceDownloaderOperation downloader;
    }
    
    public class GameStart_DownloadFiles : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            BeginDownload().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
        
        private async UniTaskVoid BeginDownload()
        {
            GameStart_DownloadFiles_Context context = GetContext<GameStart_DownloadFiles_Context>();
            
            var downloader = context.downloader;

            // 注册下载回调
            //downloader.OnDownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
            //downloader.OnDownloadProgressCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                return;

            Fsm.ChangeState(typeof(GameStart_Done));
        }
    }
}