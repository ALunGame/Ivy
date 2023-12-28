using YooAsset;

namespace IAFramework.Server
{
    public class PatchOperation : GameAsyncOperation
    {
        private GameStartServer startServer;

        public PatchOperation(string packageName, string buildPipeline, EPlayMode playMode)
        {
            startServer = new GameStartServer();
            startServer.SetData(packageName, buildPipeline, playMode);
            startServer.Init();
        }

        protected override void OnStart()
        {
            startServer.Start();
        }

        protected override void OnUpdate()
        {
            if (GameStartData.Instance.CurrState == EGameStartState.Success)
            {
                Status = EOperationStatus.Succeed;
            }
        }

        protected override void OnAbort()
        {

        }
    }
}
