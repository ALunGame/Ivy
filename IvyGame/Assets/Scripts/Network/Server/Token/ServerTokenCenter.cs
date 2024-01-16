using Game.Network.SDispatcher;
using LiteNetLib;
using Proto;
using System.Collections.Generic;

namespace Game.Network.Server
{
    /// <summary>
    /// 服务连接中心
    /// </summary>
    internal class ServerTokenCenter
    {
        private int currTokenUid = 0;

        private List<ServerToken> serverTokens = new List<ServerToken>();

        private SDispatcherMapping dispatcherMapping = new SDispatcherMapping();

        private NetProtoPacket cachedCommand = new NetProtoPacket();

        private int GenTokenUid()
        {
            currTokenUid++;
            return currTokenUid;
        }

        public void TokenEnter(NetPeer netPeer, Proto.PlayerInfo playerInfo)
        {
            TokenLeave(netPeer);

            ServerToken newToken = new ServerToken(GenTokenUid(), netPeer, playerInfo);
            netPeer.Tag = newToken;
            serverTokens.Add(newToken);

            //广播其他人有人加入
            JoinRoomS2c msg = new JoinRoomS2c();
            msg.RetCode = 0;
            foreach (ServerToken token in serverTokens)
            {
                msg.Players.Add(token.Player);
            }
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.JoinRoomS2c, msg);
        }

        public void TokenLeave(NetPeer netPeer)
        {
            ServerToken token = GetServerToken(netPeer);
            if (token != null)
            {
                serverTokens.Remove(token);

                LeaveRoomS2c msg = new LeaveRoomS2c();
                msg.playerUid = token.TokenUid;
                NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.LeaveRoomS2c, msg);
            }
        }

        public ServerToken GetServerToken(NetPeer netPeer)
        {
            return (ServerToken)netPeer.Tag;
        }

        public void OnReceiveMsg(NetPeer peer, NetPacketReader reader)
        {
            cachedCommand.Deserialize(reader);

            ushort msgId = cachedCommand.MsgId;
            string protoTypeName = cachedCommand.ProtoTypeName;
            byte[] msgData = cachedCommand.MsgData;
            dispatcherMapping.OnReceiveMsg(peer, msgId,ProtoBufTool.Decode(protoTypeName,msgData));
        }
    }
}
