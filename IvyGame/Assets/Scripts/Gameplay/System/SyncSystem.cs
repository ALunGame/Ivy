using Game.Network.Client;
using Gameplay.Player;
using IAUI;
using Proto;
using System;

namespace Gameplay.System
{
    internal class SyncSystem : GameplaySystem
    {
        protected override void OnInit()
        {
            NetClientLocate.LocalToken.AddListen<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c, OnJoinRoomS2c);
            NetClientLocate.LocalToken.AddListen<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c, OnLeaveRoomS2c);

            NetClientLocate.LocalToken.AddListen<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c, OnStartGameS2c);
            NetClientLocate.LocalToken.AddListen<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c, OnEnterMapS2c);

            NetClientLocate.LocalToken.AddListen<PlayerMoveS2c>((ushort)RoomMsgDefine.PlayerMoveS2c, OnPlayerMoveS2c);
            NetClientLocate.LocalToken.AddListen<PlayerDieS2c>((ushort)RoomMsgDefine.PlayerDieS2c, OnPlayerDieS2c);
            NetClientLocate.LocalToken.AddListen<PlayerRebornS2c>((ushort)RoomMsgDefine.PlayerRebornS2c, OnPlayerRebornS2c);
            NetClientLocate.LocalToken.AddListen<PlayerPathChangeS2c>((ushort)RoomMsgDefine.PlayerPathChangeS2c, OnPlayerPathChangeS2c);

            NetClientLocate.LocalToken.AddListen<CampAreaChangeS2c>((ushort)RoomMsgDefine.CampAreaChangeS2c, OnCampAreaChangeS2c);
            NetClientLocate.LocalToken.AddListen<GameEndS2c>((ushort)RoomMsgDefine.GameEndS2c, OnGameEndS2c);
        }

        protected override void OnClear()
        {
            NetClientLocate.LocalToken.RemoveListen<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c, OnJoinRoomS2c);
            NetClientLocate.LocalToken.RemoveListen<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c, OnLeaveRoomS2c);

            NetClientLocate.LocalToken.RemoveListen<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c, OnStartGameS2c);
            NetClientLocate.LocalToken.RemoveListen<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c, OnEnterMapS2c);

            NetClientLocate.LocalToken.RemoveListen<PlayerMoveS2c>((ushort)RoomMsgDefine.PlayerMoveS2c, OnPlayerMoveS2c);
            NetClientLocate.LocalToken.RemoveListen<PlayerDieS2c>((ushort)RoomMsgDefine.PlayerDieS2c, OnPlayerDieS2c);
            NetClientLocate.LocalToken.RemoveListen<PlayerRebornS2c>((ushort)RoomMsgDefine.PlayerRebornS2c, OnPlayerRebornS2c);
            NetClientLocate.LocalToken.RemoveListen<PlayerPathChangeS2c>((ushort)RoomMsgDefine.PlayerPathChangeS2c, OnPlayerPathChangeS2c);

            NetClientLocate.LocalToken.RemoveListen<CampAreaChangeS2c>((ushort)RoomMsgDefine.CampAreaChangeS2c, OnCampAreaChangeS2c);

            NetClientLocate.LocalToken.RemoveListen<GameEndS2c>((ushort)RoomMsgDefine.GameEndS2c, OnGameEndS2c);

        }


        #region 网络事件

        private void OnJoinRoomS2c(JoinRoomS2c MsgData)
        {
        }

        private void OnLeaveRoomS2c(LeaveRoomS2c MsgData)
        {
            GameplayLocate.GameIns.RemovePlayer(MsgData.playerUid);
        }

        private void OnStartGameS2c(StartGameS2c MsgData)
        {
            UILocate.UI.DestroyAllPanel();

            GameplayLocate.GameIns.StartGame();

            UILocate.UI.Show(UIPanelDef.FightPanel);
        }

        private void OnEnterMapS2c(EnterMapS2c MsgData)
        {
            GameplayLocate.GameIns.StartGame();
        }

        private void OnPlayerMoveS2c(PlayerMoveS2c MsgData)
        {
            GamePlayer gamePlayer = GameplayLocate.GameIns.GetPlayer(MsgData.playerUid);
            if (gamePlayer != null)
            {
                gamePlayer.UpdateMovePos(MsgData);
            }
        }

        private void OnPlayerDieS2c(PlayerDieS2c MsgData)
        {
            GameplayLocate.GameIns.GetPlayer(MsgData.diePlayerUid).Die(MsgData.rebornTime);
            GameplayLocate.GameIns.GetPlayer(MsgData.killerPlayerUid).KillCnt++;
        }

        private void OnPlayerRebornS2c(PlayerRebornS2c MsgData)
        {
            if (MsgData.RetCode == 0)
            {
                GameplayLocate.GameIns.GetPlayer(MsgData.playerUid).Reborn();
            }
        }

        private void OnPlayerPathChangeS2c(PlayerPathChangeS2c MsgData)
        {
            if (MsgData.Operate == 1)           //Add
            {
                GameplayLocate.GameIns.AddPlayerPath(MsgData.playerUid, (byte)MsgData.Pos.X, (byte)MsgData.Pos.Y);
            }
            else if (MsgData.Operate == 2)      //Remove
            {
                GameplayLocate.GameIns.RemovePlayerPath(MsgData.playerUid, (byte)MsgData.Pos.X, (byte)MsgData.Pos.Y);
            }
            else if (MsgData.Operate == 3)      //Clear
            {
                GameplayLocate.GameIns.ClearPlayerPath(MsgData.playerUid);
            }
        }

        private void OnCampAreaChangeS2c(CampAreaChangeS2c MsgData)
        {
            GameplayLocate.GameIns.ChanegMagGridCamp((byte)MsgData.Pos.X, (byte)MsgData.Pos.Y, (byte)MsgData.Camp);
        }

        private void OnGameEndS2c(GameEndS2c c)
        {
            
        }

        #endregion
    }
}
