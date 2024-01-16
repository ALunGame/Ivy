using Proto;
namespace Game.Network.CDispatcher
{
    internal class CRoomMsgDispatcher : NetClientDispatcher
    {
        internal CRoomMsgDispatcher(NetClientDispatcherMapping InMapping) : base(InMapping)
        {
            
            AddDispatch<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c,OnJoinRoomS2c);

            AddDispatch<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c,OnLeaveRoomS2c);

            AddDispatch<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c,OnStartGameS2c);

            AddDispatch<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c,OnEnterMapS2c);

            AddDispatch<PlayerMoveS2c>((ushort)RoomMsgDefine.PlayerMoveS2c,OnPlayerMoveS2c);

            AddDispatch<PlayerDieS2c>((ushort)RoomMsgDefine.PlayerDieS2c,OnPlayerDieS2c);

            AddDispatch<PlayerRebornS2c>((ushort)RoomMsgDefine.PlayerRebornS2c,OnPlayerRebornS2c);

            AddDispatch<PlayerPathChangeS2c>((ushort)RoomMsgDefine.PlayerPathChangeS2c,OnPlayerPathChangeS2c);

            AddDispatch<CampAreaChangeS2c>((ushort)RoomMsgDefine.CampAreaChangeS2c,OnCampAreaChangeS2c);

        }
        
        
        private void OnJoinRoomS2c(JoinRoomS2c MsgData)
        {











        
        
        
        
        
        
        
        
        
        
        }

        private void OnLeaveRoomS2c(LeaveRoomS2c MsgData)
        {


        
        }

        private void OnStartGameS2c(StartGameS2c MsgData)
        {











        
        
        
        
        
        
        
        
        
        
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


    }
}

