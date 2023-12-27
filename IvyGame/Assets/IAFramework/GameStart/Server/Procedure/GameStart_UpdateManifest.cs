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
    
    
    public class GameStart_UpdateManifest : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            UpdateManifest().Forget();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
        
        private async UniTaskVoid UpdateManifest()
        {
            GameStart_UpdateManifest_Context context = GetContext<GameStart_UpdateManifest_Context>();
            
            bool savePackageVersion = true;
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.UpdatePackageManifestAsync(context.updateVersion, savePackageVersion);
            await operation;
            
            if(operation.Status == EOperationStatus.Succeed)
            {
                Fsm.ChangeState(typeof(GameStart_CreateDownloader));
            }
            else
            {
                Debug.LogError(operation.Error);
            }
        }
    }
}