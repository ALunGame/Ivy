using Cysharp.Threading.Tasks;
using IAToolkit;
using UnityEngine;
using YooAsset;

namespace IAFramework.Server.Procedure
{
    public class GameStart_UpdateManifest_Context : FsmStateContext
    {
        public string updateVersion;

        public GameStart_UpdateManifest_Context(string pUpdateVersion)
        {
            updateVersion = pUpdateVersion;
        }
    }
    
    
    public class GameStart_UpdatePackageManifest : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            GameStartData.Instance.SendProcessTips("更新资源清单 !");
            UpdateManifest().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
        
        private async UniTaskVoid UpdateManifest()
        {
            await UniTask.WaitForSeconds(0.5f);

            GameStart_UpdateManifest_Context context = GetContext<GameStart_UpdateManifest_Context>();

            var packageName = Owner.PackageName;
            var packageVersion = context.updateVersion;

            var package = YooAssets.GetPackage(packageName);
            bool savePackageVersion = true;
            var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
            await operation;
            
            if(operation.Status == EOperationStatus.Succeed)
            {
                Fsm.ChangeState(typeof(GameStart_CreatePackageDownloader));
            }
            else
            {
                GameStartData.Instance.SendPopTips("补丁清单更新失败 !", operation.Error);
                Debug.LogError(operation.Error);
            }
        }
    }
}