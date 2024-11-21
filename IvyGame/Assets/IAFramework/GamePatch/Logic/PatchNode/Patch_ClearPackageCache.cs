using IAFramework.Patch;
using IAToolkit;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    /// <summary>
    /// 清理未使用的缓存文件
    /// </summary>
    internal class Patch_ClearPackageCache : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"开始清理{Owner.PackageName}未使用的缓存文件...");

            var packageName = Owner.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.ClearUnusedBundleFilesAsync();
            operation.Completed += Operation_Completed;
        }

        private void Operation_Completed(YooAsset.AsyncOperationBase obj)
        {
            GamePatchData.Instance.SendProcessTips($"清理{Owner.PackageName}未使用的缓存文件完成!!");
            Fsm.ChangeState(typeof(Patch_UpdaterDone));
        }
    }
}
