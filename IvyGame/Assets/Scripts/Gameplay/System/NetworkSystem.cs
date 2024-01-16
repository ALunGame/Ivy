using UnityEngine;
using Game.Network.Server;
using Game.Network.Client;
using System.Net;
using Game.Network;

namespace Gameplay.System
{
    internal class NetworkSystem : GameplaySystem
    {
        private NetServer Server;

        private NetClient Client;
        private bool openDiscovery = false;
        private float discoveryInterval = 1.0f; //请求间隔时间
        private float lastDiscoveryTime = 0.0f;

        protected override void OnInit()
        {
            if (GameplayLocate.GameIns.Mode.ModeType.Value == GameModeType.Local)
            {
                return;
            }

            //房主开服务器
            if (GameplayLocate.GameIns.IsRoomOwner)
            {
                Server = GameplayLocate.GameIns.gameObject.AddComponent<NetServer>();
                Server.StartServer();
            }

            //客户端
            Client = GameplayLocate.GameIns.gameObject.AddComponent<NetClient>();
            Client.SetDiscoveryCallBack((endPoint) =>
            {
                openDiscovery = false;
                Client.Connect(endPoint, null);
            });
        }

        protected override void OnStartGame()
        {
            
        }

        protected override void OnUpdate(float pDeltaTime, float pRealElapseSeconds)
        {
            if (openDiscovery)
            {
                if (pRealElapseSeconds - lastDiscoveryTime > discoveryInterval)
                {
                    SendDiscovery();
                    lastDiscoveryTime = pRealElapseSeconds;
                }
            }
        }

        private void SendDiscovery()
        {
            NetClientLocate.Log.Log("寻找服务器...", NetworkGeneral.ServerPort);
            Client.Discovery(NetworkGeneral.ServerPort);
        }
    }
}
