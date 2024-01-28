using Game.Network.Server;
using Proto;
namespace Game.Network.SDispatcher
{
    internal class SRoomMsgDispatcher : NetServerDispatcher
    {
        internal SRoomMsgDispatcher(NetServerDispatcherMapping InMapping) : base(InMapping)
        {
            
            AddDispatch<CreateRoomC2s>((ushort)RoomMsgDefine.CreateRoomC2s,OnCreateRoomC2s);

            AddDispatch<JoinRoomC2s>((ushort)RoomMsgDefine.JoinRoomC2s,OnJoinRoomC2s);

            AddDispatch<StartGameC2s>((ushort)RoomMsgDefine.StartGameC2s,OnStartGameC2s);

            AddDispatch<PlayerMoveC2s>((ushort)RoomMsgDefine.PlayerMoveC2s,OnPlayerMoveC2s);

        }
        
        
        private void OnCreateRoomC2s(LiteNetLib.NetPeer peer, CreateRoomC2s MsgData)
        {



            NetServerLocate.TokenCenter.RoomMasterPeer = peer;
            NetServerLocate.TokenCenter.RoomGameMode = MsgData.gameMode;

            CreateRoomS2c msg = new CreateRoomS2c();
            msg.RetCode = 0;
            msg.gameMode = MsgData.gameMode;
            NetServerLocate.Net.SendTo(peer,(ushort)RoomMsgDefine.CreateRoomS2c, msg);
        
        
        
        
        }

        private void OnJoinRoomC2s(LiteNetLib.NetPeer peer, JoinRoomC2s MsgData)
        {



            NetServerLocate.TokenCenter.TokenEnter(peer, MsgData.Player);
        
        
        
        }

        private void OnStartGameC2s(LiteNetLib.NetPeer peer, StartGameC2s MsgData)
        {



            NetServerLocate.StartGame(MsgData.gameCfgId, (Gameplay.GameModeType)MsgData.gameMode);
        
        
        
        }

        private void OnPlayerMoveC2s(LiteNetLib.NetPeer peer, PlayerMoveC2s MsgData)
        {



            ServerPlayer player = NetServerLocate.Game.Room.GetPlayer(MsgData.playerUid);
            player?.ChangeMoveDir(MsgData.moveDir.X, MsgData.moveDir.Y);
            player?.UpdateSpeed(0);
        
        
        
        }


    }
}

