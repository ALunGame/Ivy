using IAToolkit;

namespace IAFramework.Server.Procedure
{
    public class GameStart_Done : FsmState<GameStartServer>
    {
        protected override void OnEnter()
        {
            GameStartData.Instance.SendProcessTips("游戏初始化成功 !");
            GameStartData.Instance.SetGameStartState(EGameStartState.Success);
        }
    }
}