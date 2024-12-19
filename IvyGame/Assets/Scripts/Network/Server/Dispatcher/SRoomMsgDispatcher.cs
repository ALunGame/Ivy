using Game.Network.Server;
using Gameplay.Map;
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
            AddDispatch<GamerInputC2s>((ushort)RoomMsgDefine.GamerInputC2s,OnGamerInputC2s);
            AddDispatch<GamerSkillInputC2s>((ushort)RoomMsgDefine.GamerSkillInputC2s,OnGamerSkillInputC2s);

        }
        
        
        private void OnCreateRoomC2s(LiteNetLib.NetPeer peer, CreateRoomC2s MsgData)
        {
            NetServerLocate.GameCtrl.CreateGame(peer, MsgData);
        }

        private void OnJoinRoomC2s(LiteNetLib.NetPeer peer, JoinRoomC2s MsgData)
        {
            NetServerLocate.GameCtrl.OnJoinRoom(peer, MsgData);
        }

        private void OnStartGameC2s(LiteNetLib.NetPeer peer, StartGameC2s MsgData)
        {
            if (!IAConfig.Config.GameLevelCfg.ContainsKey(MsgData.gameCfgId))
            {
                Logger.Server?.LogError("开始游戏失败，没有对应的关卡:" + MsgData.gameCfgId);
                return;
            }

            GameLevelCfg cfg = IAConfig.Config.GameLevelCfg[MsgData.gameCfgId];

            //进入地图
            NetServerLocate.GameCtrl.EnterMap(cfg.mapId);

            //TODO:需要处理所有玩家进入以后在开始
            NetServerLocate.GameCtrl.StartGame(cfg.id);
        }

        private void OnGamerInputC2s(LiteNetLib.NetPeer peer, GamerInputC2s MsgData)
        {
            NetServerLocate.GameCtrl.OnGamerInput(peer, MsgData);
        }

        private void OnGamerSkillInputC2s(LiteNetLib.NetPeer peer, GamerSkillInputC2s MsgData)
        {
            NetServerLocate.GameCtrl.OnGamerSkillInput(peer, MsgData);
        }


    }
}

