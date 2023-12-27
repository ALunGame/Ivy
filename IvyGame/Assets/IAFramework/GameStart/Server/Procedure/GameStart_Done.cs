using IAToolkit;

namespace IAFramework.Server.Procedure
{
    public class GameStart_Done : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            Owner.OnStartSuccess?.Invoke();
        }
    }
}