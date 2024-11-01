using LiteNetLib;
using Proto;

namespace Game.Network.Server
{
    internal class BaseServerGameMode
    {
        /// <summary>
        /// 房间主人UId
        /// </summary>
        public string RoomMasterUid { get; private set; }

        #region 生命周期

        public void Init(string pRoomMasterUid, CreateRoomC2s pMsgData)
        {
            RoomMasterUid = pRoomMasterUid;
            OnInit(pMsgData);
        }

        public virtual void OnInit(CreateRoomC2s pMsgData)
        {

        }

        /// <summary>
        /// 更新逻辑
        /// </summary>
        /// <param name="pDeltaTime">间隔时间</param>
        /// <param name="pGameTime">游戏运行时间</param>
        public virtual void UpdateLogic(float pDeltaTime, float pGameTime)
        {

        }

        public void Clear()
        {
            OnClear();
        }

        public virtual void OnClear()
        {

        }

        public virtual void EnterMap(int pMapId) { }

        public virtual void StartGame(int pGameLvelId) { }

        public virtual void EndGame() { }

        #endregion

        #region 游戏规则

        public virtual void OnGamerDie(ServerGamerData pGamer)
        {

        }

        public virtual void OnGamerReborn(ServerGamerData pGamer)
        {

        }

        #endregion
    }
}
