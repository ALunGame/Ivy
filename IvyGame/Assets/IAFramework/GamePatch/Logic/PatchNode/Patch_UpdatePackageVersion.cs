using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Patch
{
    /// <summary>
    /// 更新资源版本号
    /// </summary>
    internal class Patch_UpdatePackageVersion : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"获取{Owner.PackageName}最新的资源版本...");
            UpdatePackageVersion().Forget();
        }

        private async UniTaskVoid UpdatePackageVersion()
        {
            var packageName = Owner.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();

            await operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                GamePatchData.Instance.SendPopTips($"资源{Owner.PackageName}版本号更新失败 !", operation.Error);
                Debug.LogError(operation.Error);
            }
            else
            {
                Debug.Log($"远端{Owner.PackageName}最新版本为: {operation.PackageVersion}");
                Fsm.ChangeState(typeof(Patch_UpdatePackageManifest), new Patch_UpdatePackageManifest_Context(operation.PackageVersion));
            }
        }
    }
}
