using Game.Network.Client;
using Gameplay.GameData;
using IAEngine;
using IAUI;
using Proto;
using UnityEngine;

namespace Gameplay.GameMode
{
    /// <summary>
    /// 游戏模式
    /// 1.游戏基础规则的设置和更新
    /// </summary>
    public class BaseGameMode
    {
        /// <summary>
        /// 最大玩家人数
        /// </summary>
        public int MaxPlayerCnt { get; set; }

        /// <summary>
        /// 是否要创建房间
        /// </summary>
        public bool CreateRoom { get; set; }

        private GameObject gameModeGo;
        /// <summary>
        /// 最大玩家人数
        /// </summary>
        public GameObject GameModeGo { 
            get 
            {
                if (gameModeGo == null)
                {
                    gameModeGo = new GameObject(GetType().Name);
                    gameModeGo.transform.SetParent(GameplayCtrl.Instance.transform);
                    gameModeGo.transform.Reset();
                }
                return gameModeGo;
            } 
        }

        #region 生命周期

        public void Init()
        {
            OnInit();
        }

        public virtual void OnInit()
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

        public virtual void StartGame(int pGameLevelId) { }

        public virtual void EndGame() { }

        public virtual void ExitGame() 
        {
            Clear();
        }

        #endregion

        #region 网络事件相关

        private void RegNetworkEvents()
        {
            NetClientLocate.LocalToken.AddListen<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c, OnJoinRoomS2c);
            NetClientLocate.LocalToken.AddListen<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c, OnLeaveRoomS2c);

            NetClientLocate.LocalToken.AddListen<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c, OnStartGameS2c);
            NetClientLocate.LocalToken.AddListen<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c, OnEnterMapS2c);
            NetClientLocate.LocalToken.AddListen<GameEndS2c>((ushort)RoomMsgDefine.GameEndS2c, OnGameEndS2c);

            OnRegNetworkEvents();
        }

        protected virtual void OnRegNetworkEvents()
        {

        }


        private void RemoveNetworkEvents()
        {
            NetClientLocate.LocalToken.RemoveListen<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c, OnJoinRoomS2c);
            NetClientLocate.LocalToken.RemoveListen<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c, OnLeaveRoomS2c);

            NetClientLocate.LocalToken.RemoveListen<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c, OnStartGameS2c);
            NetClientLocate.LocalToken.RemoveListen<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c, OnEnterMapS2c);

            NetClientLocate.LocalToken.RemoveListen<GameEndS2c>((ushort)RoomMsgDefine.GameEndS2c, OnGameEndS2c);

            OnRemoveNetworkEvents();
        }

        protected virtual void OnRemoveNetworkEvents()
        {

        }


        protected virtual void OnJoinRoomS2c(JoinRoomS2c MsgData)
        {
        }

        protected virtual void OnLeaveRoomS2c(LeaveRoomS2c MsgData)
        {
        }

        protected virtual void OnStartGameS2c(StartGameS2c MsgData)
        {
            UILocate.UI.DestroyAllPanel();
        }

        protected virtual void OnEnterMapS2c(EnterMapS2c MsgData)
        {
        }
        protected virtual void OnGameEndS2c(GameEndS2c c)
        {

        }

        #endregion
    }
}
