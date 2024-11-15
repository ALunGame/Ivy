using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Game.Network.Server
{
    public class NetServer : MonoBehaviour, INetEventListener
    {
        private NetManager netManager;
        private NetPacketProcessor packetProcessor;

        private Thread logicThread;
        private NetworkLogicTimer logicTimer;

        public ushort Tick {  get; private set; }

        #region 编辑器

        public void OpenLog(bool pOpen)
        {
            NetServerLocate.Log.OpenLog = pOpen;
        }

        public bool GetOpenLogState()
        {
            return NetServerLocate.Log.OpenLog;
        }

        public void OpenLogWarn(bool pOpen)
        {
            NetServerLocate.Log.OpenWarn = pOpen;
        }

        public bool GetOpenLogWarnState()
        {
            return NetServerLocate.Log.OpenWarn;
        }


        #endregion

        #region Unity线程

        private void Awake()
        {
            logicTimer = new NetworkLogicTimer(OnLogicUpdate);

            packetProcessor = new NetPacketProcessor();
            packetProcessor.SubscribeReusable<DiscoveryPacket, IPEndPoint>(OnDiscoveryReceived);
            netManager = new NetManager(this)
            {
                AutoRecycle = true,
                BroadcastReceiveEnabled = true,
            };
            //serverState = new ServerStateS2c();

            NetServerLocate.Init(this);
        }

        private void Update()
        {
            netManager.PollEvents();
            logicTimer.Update();
        }

        public Action OnDrawGizmosFunc;
        private void OnDrawGizmosSelected()
        {
            OnDrawGizmosFunc?.Invoke();
        }

        private void OnDestroy()
        {
            logicTimer.Stop();
            if (logicThread != null)
            {
                logicThread.Join();
                logicThread = null;
            }

            NetServerLocate.Clear();
            netManager.Stop();
        }

        private void OnApplicationQuit()
        {
            if (logicThread != null)
            {
                logicThread.Join();
                logicThread = null;
            }
            netManager.Stop();
        }

        #endregion

        #region 消息通信

        private NetDataWriter _cachedWriter = new NetDataWriter();
        private NetProtoPacket _cachedProtoPacket = new NetProtoPacket();

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

        /// <summary>
        /// 开始服务器监听
        /// </summary>
        public void StartServer()
        {
            if (netManager.IsRunning)
                return;
            netManager.Start(NetworkGeneral.ServerPort);
            logicTimer.Start();
            //logicThread = new Thread(UpdateThread) { Name = "NetServer.Update", IsBackground = true };
            //logicThread.Start();

            NetServerLocate.Log.Log("服务器启动成功：", NetworkGeneral.ServerPort);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void EndServer()
        {
            if (!netManager.IsRunning)
                return;
            netManager.Stop();
            if (logicThread != null)
            {
                logicThread.Join();
                logicThread = null;
            }
            NetServerLocate.Clear();
        }

        /// <summary>
        /// 机制不对
        /// </summary>
        private void UpdateThread()
        {
            while (netManager.IsRunning)
            {
                OnLogicUpdate();
            }
        }

        private void OnLogicUpdate()
        {
            Tick = (ushort)((Tick + 1) % NetworkGeneral.MaxGameSequence);
            NetServerLocate.GameCtrl.UpdateLogic();
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
                        Debug.Log("Unhandled packet: " + pt);
                        break;
                }
            }
        }

        /// <summary>
        /// 寻找服务器
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="peer"></param>
        private void OnDiscoveryReceived(DiscoveryPacket packet, IPEndPoint remoteEndPoint)
        {
            Debug.Log("[S] Discovery packet received: " + packet.DiscoveryStr);

            _cachedWriter.Reset();
            _cachedWriter.Put((byte)PacketType.Discovery);
            packetProcessor.Write(_cachedWriter, new DiscoveryPacket { DiscoveryStr = "SDiscovery" });
            netManager.SendUnconnectedMessage(_cachedWriter, remoteEndPoint);
        }

        /// <summary>
        /// 客户端请求链接
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(NetworkGeneral.NetConnectKey);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            NetServerLocate.Log.Log("[S] Player connected: ", peer.EndPoint);
            NetServerLocate.TokenCenter.TokenEnter(peer);
        }

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

        /// <summary>
        /// 更新延迟信息
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="latency"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }
    }
}
