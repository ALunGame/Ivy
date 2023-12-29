using IAToolkit;

namespace IAFramework.Server.Procedure
{
    public class GameStart_DownloadPackageOver : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            Fsm.ChangeState(typeof(GameStart_ClearPackageCache));
        }
    }
}
