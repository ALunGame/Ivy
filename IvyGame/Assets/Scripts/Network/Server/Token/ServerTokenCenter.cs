using Game.Network.SDispatcher;
using LiteNetLib;
using Proto;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    #region Delegate

    internal delegate void OnUpdate(float pDeltaTime, float pRealElapseSeconds);

    #endregion

    /// <summary>
    /// 服务连接中心
    /// </summary>
    internal class ServerTokenCenter
    {
        private int currTokenUid = 0;

        private List<ServerToken> serverTokens = new List<ServerToken>();

        private SDispatcherMapping dispatcherMapping = new SDispatcherMapping();

        private NetProtoPacket cachedCommand = new NetProtoPacket();

        /// <summary>
        /// 帧已经运行秒数
        /// </summary>
        public float UpdateRealElapseSeconds { get; private set; }
        /// <summary>
        /// 每帧间隔时间
        /// </summary>
        public float UpdateDeltaTime { get; private set; }
        /// <summary>
        /// 每帧时间缩放
        /// </summary>
        public float UpdateTimeScale { get; private set; }

        public event OnUpdate Evt_OnUpdate;

        public ServerTokenCenter()
        {
            UpdateRealElapseSeconds = 0;
            UpdateDeltaTime = Time.deltaTime;
            UpdateTimeScale = Time.timeScale;
        }

        public void Update()
        {
            UpdateRealElapseSeconds += UpdateDeltaTime * UpdateTimeScale;
            Evt_OnUpdate?.Invoke(UpdateDeltaTime * UpdateTimeScale, UpdateRealElapseSeconds);
        }

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

                //广播其他人有人离开
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
