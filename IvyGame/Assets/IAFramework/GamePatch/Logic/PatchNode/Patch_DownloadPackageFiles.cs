using Cysharp.Threading.Tasks;
using IAFramework.Server.Procedure;
using IAToolkit;
using YooAsset;

namespace IAFramework.Patch
{
    public class Patch_DownloadPackageFiles_Context : FsmStateContext
    {
        public ResourceDownloaderOperation downloader;
    }

    /// <summary>
    /// 下载更新文件
    /// </summary>
    internal class Patch_DownloadPackageFiles : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"开始下载{Owner.PackageName}补丁文件...");
            BeginDownload().Forget();
        }

        private async UniTaskVoid BeginDownload()
        {
            Patch_DownloadPackageFiles_Context context = GetContext<Patch_DownloadPackageFiles_Context>();

            var downloader = context.downloader;
            downloader.OnDownloadErrorCallback = OnDownloadError;
            downloader.OnDownloadProgressCallback = OnDownloadFiles;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
            {
                GamePatchData.Instance.SendPopTips($"下载{Owner.PackageName}补丁文件失败 !", downloader.Error);
                return;
            }

            Fsm.ChangeState(typeof(Patch_DownloadPackageOver));
        }

        private void OnDownloadError(string fileName, string error)
        {

        }

        public void OnDownloadFiles(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {

        }
    }
}
