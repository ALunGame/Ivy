using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Patch
{
    /// <summary>
    /// 创建文件下载器
    /// </summary>
    internal class Patch_CreatePackageDownloader : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"创建{Owner.PackageName}补丁下载器...");
            CreateDownloader();
        }

        private void CreateDownloader()
        {
            var packageName = Owner.PackageName;
            var package = YooAssets.GetPackage(packageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (downloader.TotalDownloadCount == 0)
            {
                Fsm.ChangeState(typeof(Patch_UpdaterDone));
            }
            else
            {
                Patch_DownloadPackageFiles_Context context = new Patch_DownloadPackageFiles_Context();
                context.downloader = downloader;

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;

                Debug.Log($"开始热更{Owner.PackageName}文件->数量:{totalDownloadCount} 大小:{totalDownloadBytes}");
                Fsm.ChangeState(typeof(Patch_DownloadPackageFiles), context);
            }
        }
    }
}
