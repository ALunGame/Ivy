using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Game.Network.Client
{
    public class NetClient : MonoBehaviour, INetEventListener
    {
        private NetManager netManager;
        
        private NetPacketProcessor packetProcessor;

        private NetPeer netServer;
        private Action onConnected;
        private Action<DisconnectInfo> onDisconnected;

        internal NetworkLogicTimer LogicTimer { get; private set; }

        private int ping;

        #region Unity

        private void Awake()
        {
            packetProcessor = new NetPacketProcessor();
            packetProcessor.SubscribeReusable<DiscoveryPacket, IPEndPoint>(OnDiscoveryReceived);

            netManager = new NetManager(this)
            {
                AutoRecycle = true,
                IPv6Enabled = false,
                UnconnectedMessagesEnabled = true,
                BroadcastReceiveEnabled = true,
                DisconnectTimeout = 500000000,          //TODO:临时
            };
            netManager.Start();

            LogicTimer = new NetworkLogicTimer(OnLogicUpdate);

            NetClientLocate.Init(this);
        }

        private void Update()
        {
            netManager.PollEvents();
            LogicTimer.Update();
        }

        private void OnLogicUpdate()
        {
            NetClientLocate.OnNetWorkLogicUpdate();
        }

        private void OnDestroy()
        {
            netManager.Stop();
            LogicTimer.Stop();
            NetworkEvent.Clear();
        }

        private void OnApplicationQuit()
        {
            netManager.Stop();
            LogicTimer.Stop();
            NetworkEvent.Clear();
        }

        #endregion


        #region 编辑器

        public void OpenLog(bool pOpen)
        {
            NetClientLocate.Log.OpenLog = pOpen;
        }

        public bool GetOpenLogState()
        {
            return NetClientLocate.Log.OpenLog;
        }

        public void OpenLogWarn(bool pOpen)
        {
            NetClientLocate.Log.OpenWarn = pOpen;
        }

        public bool GetOpenLogWarnState()
        {
            return NetClientLocate.Log.OpenWarn;
        }


        #endregion

        #region 广播用于发现服务器

        private Action<IPEndPoint> onDiscoveryServer;
        public void Discovery(int port)
        {
            if (netManager == null)
            {
                NetClientLocate.Log.Log("Discovery000...", NetworkGeneral.ServerPort);
                return;
            }
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.Discovery);
            packetProcessor.Write(_cachedWriter, new DiscoveryPacket { DiscoveryStr = "CDiscovery" });
            netManager.SendBroadcast(_cachedWriter, port);
        }

        public void SetDiscoveryCallBack(Action<IPEndPoint> onDiscoveryCallBack)
        {
            onDiscoveryServer = onDiscoveryCallBack;
        }

        /// <summary>
        /// 寻找服务器
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="peer"></param>
        private void OnDiscoveryReceived(DiscoveryPacket packet, IPEndPoint remoteEndPoint)
        {
            NetClientLocate.Log.Log("发现服务器》》》", remoteEndPoint, packet.DiscoveryStr);
            onDiscoveryServer?.Invoke(remoteEndPoint);
        }

        #endregion

        #region 消息通信

        private NetDataWriter _cachedWriter = new NetDataWriter();
        private NetProtoPacket _cachedProtoPacket = new NetProtoPacket();

        public void Send<T>(ushort msgId, T msgData, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : IExtensible
        {
            if (netServer == null)
            {
                return;
            }

            _cachedProtoPacket.PutMsgId(msgId);
            _cachedProtoPacket.PutProtoTypeName(msgData.GetType().FullName);
            _cachedProtoPacket.PutMsgData(ProtoBufTool.Encode(msgData));

            NetClientLocate.Log.Log($"<color=#008EFF>Send:{msgId}->{msgData.GetType().FullName}</color>");

            netServer.Send(WriteSerializable(PacketType.Proto, _cachedProtoPacket), deliveryMethod);
        }

        private NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
        {
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)type);
            packet.Serialize(_cachedWriter);
            return _cachedWriter;
        }

        #endregion

        public void Connect(IPEndPoint endPoint, Action onConnected, Action<DisconnectInfo> onDisconnected)
        {
            this.onConnected = onConnected;
            this.onDisconnected = onDisconnected;
            netManager.Connect(endPoint, NetworkGeneral.NetConnectKey);
        }

        /// <summary>
        /// 连接服务器成功
        /// </summary>
        /// <param name="peer"></param>
        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] Connected to server: " + peer.EndPoint);
            netServer = peer;

            onConnected?.Invoke();
            LogicTimer.Start();
        }

        /// <summary>
        /// 连接服务器失败
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="disconnectInfo"></param>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[C] Disconnected from server: " + disconnectInfo.Reason);
            netServer = null;
            if (onDisconnected != null)
            {
                onDisconnected(disconnectInfo);
                onDisconnected = null;
            }
            LogicTimer.Stop();
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
                    NetClientLocate.LocalToken.OnReceiveMsg(peer, reader);
                    break;
                default:
                    Debug.Log("Unhandled packet: " + pt);
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
            if (messageType == UnconnectedMessageType.BasicMessage)
            {
                byte packetType = reader.GetByte();
                if (packetType >= NetworkGeneral.PacketTypesCount)
                    return;

                PacketType pt = (PacketType)packetType;
                switch (pt)
                {
                    case PacketType.Discovery:
                        packetProcessor.ReadAllPackets(reader, remoteEndPoint);
                        break;
                    default:
                        Debug.Log("Unhandled packet: " + pt);
                        break;
                }
            }
        }


        /// <summary>
        /// 网络出错
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="socketError"></param>
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }
    }
}
