using Game.Network.Client;
using Gameplay;
using IAEngine;
using Proto;
namespace Game.Network.CDispatcher
{
    internal class CRoomMsgDispatcher : NetClientDispatcher
    {
        internal CRoomMsgDispatcher(NetClientDispatcherMapping InMapping) : base(InMapping)
        {
            
            AddDispatch<CreateRoomS2c>((ushort)RoomMsgDefine.CreateRoomS2c,OnCreateRoomS2c);

            AddDispatch<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c,OnJoinRoomS2c);

            AddDispatch<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c,OnLeaveRoomS2c);

            AddDispatch<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c,OnStartGameS2c);

            AddDispatch<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c,OnEnterMapS2c);

            AddDispatch<PlayerMoveS2c>((ushort)RoomMsgDefine.PlayerMoveS2c,OnPlayerMoveS2c);

            AddDispatch<PlayerDieS2c>((ushort)RoomMsgDefine.PlayerDieS2c,OnPlayerDieS2c);

            AddDispatch<PlayerRebornS2c>((ushort)RoomMsgDefine.PlayerRebornS2c,OnPlayerRebornS2c);

            AddDispatch<PlayerPathChangeS2c>((ushort)RoomMsgDefine.PlayerPathChangeS2c,OnPlayerPathChangeS2c);

            AddDispatch<CampAreaChangeS2c>((ushort)RoomMsgDefine.CampAreaChangeS2c,OnCampAreaChangeS2c);

            AddDispatch<GameEndS2c>((ushort)RoomMsgDefine.GameEndS2c,OnGameEndS2c);

            AddDispatch<RoomMembersChangeS2c>((ushort)RoomMsgDefine.RoomMembersChangeS2c,OnRoomMembersChangeS2c);

        }
        
        
        private void OnCreateRoomS2c(CreateRoomS2c MsgData)
        {
            GameplayLocate.UserData.Room.ChangeGameMode((GameModeType)MsgData.gameMode);

            //发送加入
            JoinRoomC2s data = new JoinRoomC2s();
            data.Player = new JoinPlayerInfo();
            data.Player.Name = "zzz";
            data.Player.Id = 1;
            NetClientLocate.Log.LogWarning("发送加入>>>>", "zzz");
            NetClientLocate.Net.Send((ushort)RoomMsgDefine.JoinRoomC2s, data);
        }

        private void OnJoinRoomS2c(JoinRoomS2c MsgData)
        {
            GameplayLocate.UserData.Room.ChangeGameMode((GameModeType)MsgData.gameMode);
            GameplayLocate.UserData.Room.SetRoomMasterUid(MsgData.roomMasterPlayerUid);
            GameplayLocate.UserData.Room.SetSelfPlayerId(MsgData.selfPlayerUid);

            NetClientLocate.LocalToken.SetLocalPlayerUid(MsgData.selfPlayerUid);           
        }

        private void OnLeaveRoomS2c(LeaveRoomS2c MsgData)
        {
            GameplayLocate.UserData.Room.RemovePlayerInfo(MsgData.playerUid);
        }

        private void OnStartGameS2c(StartGameS2c MsgData)
        {
            if (MsgData.Players.IsLegal())
            {
                for (int i = 0; i < MsgData.Players.Count; i++)
                {
                    GameplayLocate.UserData.Game.UpdatePlayerInfo(MsgData.Players[i]);
                }
            }
        }

        private void OnEnterMapS2c(EnterMapS2c MsgData)
        {
        }

        private void OnPlayerMoveS2c(PlayerMoveS2c MsgData)
        {
        }

        private void OnPlayerDieS2c(PlayerDieS2c MsgData)
        {
        }

        private void OnPlayerRebornS2c(PlayerRebornS2c MsgData)
        {
        }

        private void OnPlayerPathChangeS2c(PlayerPathChangeS2c MsgData)
        {
        }

        private void OnCampAreaChangeS2c(CampAreaChangeS2c MsgData)
        {
        }

        private void OnGameEndS2c(GameEndS2c MsgData)
        {}

        private void OnRoomMembersChangeS2c(RoomMembersChangeS2c MsgData)
        {
            if (MsgData.Players.IsLegal())
            {
                for (int i = 0; i < MsgData.Players.Count; i++)
                {
                    GameplayLocate.UserData.Room.UpdatePlayerInfo(MsgData.Players[i]);
                }
            }
        }


    }
}

