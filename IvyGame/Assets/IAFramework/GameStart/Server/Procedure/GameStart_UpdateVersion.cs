using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_UpdateVersion : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            GetStaticVersion().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        private async UniTaskVoid GetStaticVersion()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.UpdatePackageVersionAsync();
            await operation;
            
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"远端最新版本为: {operation.PackageVersion}");
                Fsm.ChangeState(typeof(GameStart_UpdateManifest),new GameStart_UpdateManifest_Context(operation.PackageVersion));
            }
            else
            {
                Debug.LogError(operation.Error);
            }
        }
    }
}