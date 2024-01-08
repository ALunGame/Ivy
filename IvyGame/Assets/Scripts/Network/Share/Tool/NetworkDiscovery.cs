using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Game.Network
{
    /// <summary>
    /// 网络发现
    /// </summary>
    internal class NetDiscovery : MonoBehaviour
    {
        
        private UdpClient udpClient;
        
        private float discoveryInterval = 5.0f; //请求间隔时间
        private float lastDiscoveryTime = 0.0f;

        private bool openDiscovery = false;
        private int discoveryPort = 10515;
        private List<IPEndPoint> discoveredServers = new List<IPEndPoint>();
        private Action<IPEndPoint> onDiscoveryServer;

        private void Start()
        {
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void Update()
        {
            if (!openDiscovery) 
            {
                return;
            }

            if (Time.time - lastDiscoveryTime > discoveryInterval)
            {
                SendDiscoveryRequest();
                lastDiscoveryTime = Time.time;
            }
        }

        private void SendDiscoveryRequest()
        {
            IPEndPoint everyone = new IPEndPoint(IPAddress.Broadcast, discoveryPort);
            byte[] sendBytes = Encoding.UTF8.GetBytes("ClientDiscovery");

            udpClient.Send(sendBytes, sendBytes.Length, everyone);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, discoveryPort);
            byte[] receivedBytes = udpClient.EndReceive(ar, ref serverEndPoint);

            string receivedString = Encoding.UTF8.GetString(receivedBytes);
            Debug.Log($"发现一个服务器: {serverEndPoint.Address} - 消息: {receivedString}");

            if (!discoveredServers.Exists(ep => ep.Address.Equals(serverEndPoint.Address) && ep.Port == serverEndPoint.Port))
            {
                discoveredServers.Add(serverEndPoint);
                onDiscoveryServer?.Invoke(serverEndPoint);
            }

            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void OnDestroy()
        {
            udpClient.Close();
            udpClient.Dispose();
        }

        private void OnApplicationQuit()
        {
            udpClient.Close();
            udpClient.Dispose();
        }


        public void StartDiscovery(int port, Action<IPEndPoint> onDiscoveryCallBack)
        {
            discoveryPort = port;
            discoveredServers.Clear();
            onDiscoveryServer = onDiscoveryCallBack;
            openDiscovery = true;
        }

        public void StopDiscovery()
        {
            openDiscovery = false;
        }
    }
}