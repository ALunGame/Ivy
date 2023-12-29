using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_CreatePackageDownloader : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips("创建补丁下载器 !");
            CreateDownloader().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        private async UniTaskVoid CreateDownloader()
        {
            await UniTask.WaitForSeconds(0.5f);

            var packageName = Owner.PackageName;
            var package = YooAssets.GetPackage(packageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("Not found any download files !");
                Fsm.ChangeState(typeof(GameStart_Done));
            }
            else
            {
                GameStart_DownloadFiles_Context context = new GameStart_DownloadFiles_Context();
                context.downloader = downloader;
                
                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                Fsm.ChangeState(typeof(GameStart_DownloadPackageFiles), context);
            }
        }
    }
}