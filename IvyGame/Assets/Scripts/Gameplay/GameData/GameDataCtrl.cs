using Proto;

namespace Gameplay.GameData
{
    /// <summary>
    /// 游戏数据层
    /// 1，目前没有依据不同的GameMode创建不同的数据
    ///     如果创建不同的数据，这样会让事情变得复杂，也会让数据冗余或者不一致
    /// </summary>
    public class GameDataCtrl : GameplayProcess
    {
        /// <summary>
        /// 服务器帧
        /// </summary>
        public ushort ServerTick { get; private set; }

        /// <summary>
        /// 服务器上一次处理的命令Id
        /// </summary>
        public ushort LastProcessedCommand { get; private set; }

        /// <summary>
        /// 房主玩家Id
        /// </summary>
        public string RoomMasterUid { get; set; }

        /// <summary>
        /// 自己的玩家Id
        /// </summary>
        public string SelfGamerUid { get; set; }

        public GameMapData Map {  get; private set; }

        public GameGamersData Gamers {  get; private set; }

        public override void OnInit()
        {
            Map = new GameMapData();
            Gamers = new GameGamersData();
        }

        public override void OnUpdateLogic(float pDeltaTime, float pGameTime)
        {
            Map.UpdateLogic(pDeltaTime, pGameTime);
            Gamers.UpdateLogic(pDeltaTime, pGameTime);
        }

        public override void OnClear()
        {
            Map.Clear();
            Map = null;

            Gamers.Clear();
            Gamers = null;
        }

        public override void OnEnterMap(int pMapId)
        {
            Map.OnEnterMap(pMapId);
        }

        public override void OnStartGame(int pGameLevelId)
        {
            Map.OnStartGame(pGameLevelId);
        }

        public void OnReceiveServerState(ServerStateS2c pMsgData)
        {
            for (int i = 0; i < pMsgData.gamerStates.Count; i++)
            {
                GamerBaseState gamerState = pMsgData.gamerStates[i];
                GamerData gamerData = Gamers.GetGamer(gamerState.gamerUid);
                if (gamerData != null) 
                {
                    gamerData.OnReceiveServerStateMsg(pMsgData, gamerState);
                }
            }
        }

        public bool CheckIsRoomMaster(string pGamerUid = "-1")
        {
            if (pGamerUid == "-1")
            {
                return SelfGamerUid == RoomMasterUid;
            }
            return RoomMasterUid == pGamerUid;
        }
    }
}
