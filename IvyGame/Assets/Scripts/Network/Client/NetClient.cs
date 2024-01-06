using Game.Network.Server;
using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Game.Network.Client
{
    internal class NetClient : MonoBehaviour, INetEventListener
    {
        private NetManager netManager;
        
        private NetPacketProcessor packetProcessor;

        private NetPeer netServer;
        private Action<DisconnectInfo> onDisconnected;

        private string userName;

        #region Unity

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            System.Random r = new System.Random();
            userName = Environment.MachineName + " " + r.Next(100000);

            packetProcessor = new NetPacketProcessor();
            packetProcessor.SubscribeReusable<DiscoveryPacket, IPEndPoint>(OnDiscoveryReceived);

            netManager = new NetManager(this)
            {
                AutoRecycle = true,
                IPv6Enabled = false,
                UnconnectedMessagesEnabled = true,
            };
            netManager.Start();

            NetClientLocate.Init(this);
        }

        private void Update()
        {
            netManager.PollEvents();
        }

        private void OnDestroy()
        {
            netManager.Stop();
        }

        #endregion

        #region 广播用于发现服务器

        private Action<IPEndPoint> onDiscoveryServer;
        public void Discovery(int port)
        {
            if (netManager == null)
            {
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

        private readonly NetDataWriter _cachedWriter = new NetDataWriter();
        private readonly NetProtoPacket _cachedProtoPacket = new NetProtoPacket();

        public void Send<T>(ushort msgId, T msgData, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : IExtensible
        {
            if (netServer == null)
            {
                return;
            }

            _cachedProtoPacket.PutMsgId(msgId);
            _cachedProtoPacket.PutProtoTypeName(typeof(T).Name);
            _cachedProtoPacket.PutMsgData(ProtoBufTool.Encode(msgData));

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


        public void Connect(IPEndPoint endPoint, Action<DisconnectInfo> onDisconnected)
        {
            this.onDisconnected = onDisconnected;
            netManager.Connect(endPoint, NetServer.NetConnectKey);
        }


        /// <summary>
        /// 连接服务器成功
        /// </summary>
        /// <param name="peer"></param>
        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[C] Connected to server: " + peer.EndPoint);
            netServer = peer;

            //发送加入
            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.JoinRoom);
            packetProcessor.Write(_cachedWriter, new JoinPacket { UserName = userName });
            netServer.Send(_cachedWriter, DeliveryMethod.ReliableOrdered);
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
                case PacketType.JoinRoom:
                    packetProcessor.ReadAllPackets(reader, peer);
                    break;
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
        }
    }
}
