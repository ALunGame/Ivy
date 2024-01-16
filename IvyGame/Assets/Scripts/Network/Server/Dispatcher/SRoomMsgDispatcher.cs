using Game.Network.Server;
using Proto;
namespace Game.Network.SDispatcher
{
    internal class SRoomMsgDispatcher : NetServerDispatcher
    {
        internal SRoomMsgDispatcher(NetServerDispatcherMapping InMapping) : base(InMapping)
        {
            
            AddDispatch<JoinRoomC2s>((ushort)RoomMsgDefine.JoinRoomC2s,OnJoinRoomC2s);

            AddDispatch<StartGameC2s>((ushort)RoomMsgDefine.StartGameC2s,OnStartGameC2s);

            AddDispatch<PlayerMoveC2s>((ushort)RoomMsgDefine.PlayerMoveC2s,OnPlayerMoveC2s);

        }
        
        
        private void OnJoinRoomC2s(LiteNetLib.NetPeer peer, JoinRoomC2s MsgData)
        {
            NetServerLocate.TokenCenter.TokenEnter(peer, MsgData.Player);
        }

        private void OnStartGameC2s(LiteNetLib.NetPeer peer, StartGameC2s MsgData)
        {


        
        }

        private void OnPlayerMoveC2s(LiteNetLib.NetPeer peer, PlayerMoveC2s MsgData)
        {


        
        }


    }
}

