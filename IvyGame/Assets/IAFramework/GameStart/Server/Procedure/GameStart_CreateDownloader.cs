using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_CreateDownloader : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            CreateDownloader();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        private void CreateDownloader()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            
            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("Not found any download files !");
                Fsm.ChangeState(typeof(GameStart_ClearAssetCache));
            }
            else
            {
                GameStart_DownloadFiles_Context context = new GameStart_DownloadFiles_Context();
                context.downloader = downloader;
                
                //A total of 10 files were found that need to be downloaded
                Debug.Log($"Found total {downloader.TotalDownloadCount} files that need download ！");

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                Fsm.ChangeState(typeof(GameStart_DownloadFiles));
            }
        }
    }
}