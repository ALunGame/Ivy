using IAToolkit;

namespace IAFramework.Server.Procedure
{
    public class GameStart_ClearAssetCache : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            var package = YooAsset.YooAssets.GetPackage("DefaultPackage");
            var operation = package.ClearUnusedCacheFilesAsync();
            operation.Completed += Operation_Completed;
        }
        
        private void Operation_Completed(YooAsset.AsyncOperationBase obj)
        {
            Fsm.ChangeState(typeof(GameStart_Done));
        }
    }
}