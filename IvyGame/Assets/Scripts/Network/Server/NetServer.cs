using IAConfig;
using IAEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;
using System;
using System.Net;
using System.Net.Sockets;

namespace Game.Network.Server
{
    public class NetServer : INetEventListener
    {
        private NetManager netManager;
        private NetPacketProcessor packetProcessor;
        private NetworkLogicTimer logicTimer;
        private readonly object _activeLock = new object();
        private bool isActive = false;

        private NetDataWriter _cachedWriter = new NetDataWriter();
        private NetProtoPacket _cachedProtoPacket = new NetProtoPacket();
        private bool updateInThread;

        public ushort Tick { get; private set; }
        public bool IsActive { get => isActive; }

        public void Init(bool pUpdateInThread)
        {
            packetProcessor = new NetPacketProcessor();
            packetProcessor.SubscribeReusable<DiscoveryPacket, IPEndPoint>(OnDiscoveryReceived);

            logicTimer = new NetworkLogicTimer(OnLogicUpdate);
            netManager = new NetManager(this)
            {
                AutoRecycle = true,
                BroadcastReceiveEnabled = true,
                DisconnectTimeout = 100 * 1000,
            };

            updateInThread = pUpdateInThread;

            Config.Preload();
        }

        public void Start()
        {
            SetActive(true);

            if (netManager.IsRunning)
                return;

            netManager.Start(NetworkGeneral.ServerPort);
            logicTimer.Start();

            if (updateInThread)
            {
                TaskHelper.AddTask(() =>
                {
                    while (isActive)
                    {
                        Update();
                    }
                }, () =>
                {
                });
            }
        }

        public void Update()
        {
            netManager.PollEvents();
            logicTimer.Update();
        }

        private void OnLogicUpdate()
        {
            Tick = (ushort)((Tick + 1) % NetworkGeneral.MaxGameSequence);
            NetServerLocate.GameCtrl.UpdateLogic();
        }

        public void Stop()
        {
            SetActive(false);

            if (!netManager.IsRunning)
                return;

            NetServerLocate.Clear();
            netManager.Stop();
        }

        private void SetActive(bool pIsActive)
        {
            lock (_activeLock)
            {
                isActive = pIsActive;
            }
        }

        #region 消息发送

        /// <summary>
        /// 消息广播
        /// </summary>
        public void Broadcast<T>(ushort msgId, T msgData, NetPeer exPeer = null, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : IExtensible
        {
            _cachedProtoPacket.PutMsgId(msgId);
            _cachedProtoPacket.PutProtoTypeName(msgData.GetType().FullName);
            _cachedProtoPacket.PutMsgData(ProtoBufTool.Encode(msgData));

            NetServerLocate.Log.Log($"<color=#FFBF00>Broadcast:{msgId}->{msgData.GetType().FullName}</color>");
            netManager.SendToAll(WriteSerializable(PacketType.Proto, _cachedProtoPacket), deliveryMethod, exPeer);
        }

        /// <summary>
        /// 发送到指定客户端
        /// </summary>
        public void SendTo<T>(NetPeer peer, ushort msgId, T msgData, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : IExtensible
        {
            _cachedProtoPacket.PutMsgId(msgId);
            _cachedProtoPacket.PutProtoTypeName(msgData.GetType().FullName);
            _cachedProtoPacket.PutMsgData(ProtoBufTool.Encode(msgData));

            NetServerLocate.Log.Log($"<color=#FFBF00>SendTo:{msgId}->{msgData.GetType().FullName}</color>");
            peer.Send(WriteSerializable(PacketType.Proto, _cachedProtoPacket), deliveryMethod);
        }

        private NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)type);
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }

        #endregion

        #region 消息接收

        /// <summary>
        /// 当收到客户端广播消息
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="peer"></param>
        private void OnDiscoveryReceived(DiscoveryPacket packet, IPEndPoint remoteEndPoint)
        {
            NetServerLocate.Log.Log("服务器发现客户端：" + packet.DiscoveryStr);

            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.Discovery);
            packetProcessor.Write(_cachedWriter, new DiscoveryPacket { DiscoveryStr = "SDiscovery" });
            netManager.SendUnconnectedMessage(_cachedWriter, remoteEndPoint);
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="channelNumber"></param>
        /// <param name="deliveryMethod"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.GetByte();
            if (packetType >= NetworkGeneral.PacketTypesCount)
                return;

            PacketType pt = (PacketType)packetType;
            switch (pt)
            {
                case PacketType.Proto:
                    NetServerLocate.TokenCenter.OnReceiveMsg(peer, reader);
                    break;
                default:
                    NetServerLocate.Log.Log("Unhandled packet: " + pt);
                    break;
            }
        }

        /// <summary>
        /// 当没有连接的时候，接受到消息
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="reader"></param>
        /// <param name="messageType"></param>
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.Broadcast)
            {
                byte packetType = reader.GetByte();
                if (packetType >= NetworkGeneral.PacketTypesCount)
                    return;

                PacketType pt = (PacketType)packetType;
                switch (pt)
                {
                    case PacketType.Discovery:
                        NetServerLocate.Log.Log("客户端寻找服务器消息》》》", remoteEndPoint);
                        packetProcessor.ReadAllPackets(reader, remoteEndPoint);
                        break;
                    default:
                        NetServerLocate.Log.Log("Unhandled packet: " + pt);
                        break;
                }
            }
        }

        /// <summary>
        /// 更新延迟信息
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="latency"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        #endregion

        #region 连接状态

        /// <summary>
        /// 客户端请求链接
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(NetworkGeneral.NetConnectKey);
        }

        /// <summary>
        /// 客户端连接成功
        /// </summary>
        /// <param name="peer"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnPeerConnected(NetPeer peer)
        {
            NetServerLocate.Log.Log("[S] Player connected: ", peer.EndPoint);
            NetServerLocate.TokenCenter.TokenEnter(peer);
        }

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="disconnectInfo"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            NetServerLocate.Log.Log("[S] Player disconnected: ", peer.EndPoint, disconnectInfo.Reason);
            NetServerLocate.TokenCenter.TokenLeave(peer);
        }

        /// <summary>
        /// 网络出错
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="socketError"></param>
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            NetServerLocate.Log.LogError("[S] NetworkError: ", endPoint, socketError);
        } 

        #endregion
    }
}
