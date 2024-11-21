using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Patch
{
    public class Patch_UpdatePackageManifest_Context : FsmStateContext
    {
        public string updateVersion;

        public Patch_UpdatePackageManifest_Context(string pUpdateVersion)
        {
            updateVersion = pUpdateVersion;
        }
    }

    /// <summary>
    /// 更新资源清单
    /// </summary>
    internal class Patch_UpdatePackageManifest : FsmState<PatchOperation>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips($"更新{Owner.PackageName}资源清单...");
            UpdateManifest().Forget();
        }

        private async UniTaskVoid UpdateManifest()
        {
            var packageName = Owner.PackageName;

            Patch_UpdatePackageManifest_Context context = GetContext<Patch_UpdatePackageManifest_Context>();
            var packageVersion = context.updateVersion;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageManifestAsync(packageVersion);

            await operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                GamePatchData.Instance.SendPopTips($"补丁{Owner.PackageName}清单更新失败 !", operation.Error);
                Debug.LogError(operation.Error);
            }
            else
            {
                Fsm.ChangeState(typeof(Patch_CreatePackageDownloader));
            }
        }
    }
}
