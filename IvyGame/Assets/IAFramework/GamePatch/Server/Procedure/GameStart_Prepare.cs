using IAToolkit;

namespace IAFramework.Server.Procedure
{
    public class GameStart_Prepare : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            Fsm.ChangeState(typeof(GameStart_Initialize));
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
    }
}