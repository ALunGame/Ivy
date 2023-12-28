using IAToolkit;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_ClearPackageCache : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            GameStartData.Instance.SendProcessTips("清理未使用的缓存文件 !");

            var packageName = Owner.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.ClearUnusedCacheFilesAsync();
            operation.Completed += Operation_Completed;
        }

        private void Operation_Completed(AsyncOperationBase @base)
        {
            Fsm.ChangeState(typeof(GameStart_Done));
        }
    }
}
