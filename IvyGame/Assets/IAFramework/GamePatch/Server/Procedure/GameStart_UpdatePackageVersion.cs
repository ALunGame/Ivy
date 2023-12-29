using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_UpdatePackageVersion : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            GamePatchData.Instance.SendProcessTips("获取最新的资源版本 !");
            UpdatePackageVersion().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        private async UniTaskVoid UpdatePackageVersion()
        {
            await UniTask.WaitForSeconds(0.5f);

            var packageName = Owner.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageVersionAsync();

            await operation;
            
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"远端最新版本为: {operation.PackageVersion}");
                Fsm.ChangeState(typeof(GameStart_UpdatePackageManifest),new GameStart_UpdateManifest_Context(operation.PackageVersion));
            }
            else
            {
                GamePatchData.Instance.SendPopTips("资源版本号更新失败 !", operation.Error);
                Debug.LogError(operation.Error);
            }
        }
    }
}