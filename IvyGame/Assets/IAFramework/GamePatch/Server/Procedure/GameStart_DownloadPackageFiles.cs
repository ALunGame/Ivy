using Cysharp.Threading.Tasks;
using IAToolkit;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_DownloadFiles_Context : FsmStateContext
    {
        public ResourceDownloaderOperation downloader;
    }
    
    public class GameStart_DownloadPackageFiles : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips("开始下载补丁文件 !");
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
            downloader.OnDownloadErrorCallback = OnDownloadError;
            downloader.OnDownloadProgressCallback = OnDownloadFiles;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
            {
                GamePatchData.Instance.SendPopTips("下载失败 !", downloader.Error);
                return;
            }
                
            Fsm.ChangeState(typeof(GameStart_DownloadPackageOver));
        }

        private void OnDownloadError(string fileName, string error)
        {

        }

        public void OnDownloadFiles(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {

        }
    }
}