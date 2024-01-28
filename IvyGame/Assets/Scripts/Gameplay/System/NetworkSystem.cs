using Game.Network;
using Game.Network.Client;
using Game.Network.Server;
using Proto;
using UnityEngine;

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
            if (GameplayLocate.GameIns.Mode.ModeType == GameModeType.Local)
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
                Client.Connect(endPoint, OnConnectServer, null);
            });
            openDiscovery = true;
            SendDiscovery();
        }

        protected override void OnStartGame()
        {

        }

        protected override void OnClear()
        {
            if (Server != null)
            {
                GameObject.Destroy(Server.gameObject);
            }
            if (Client != null)
            {
                GameObject.Destroy(Client.gameObject);
            }
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

        private void OnConnectServer()
        {
            //发送创建房间
            if (GameplayLocate.GameIns.IsRoomOwner)
            {
                CreateRoomC2s createRoomMsg = new CreateRoomC2s();
                createRoomMsg.gameMode = (int)GameplayLocate.GameIns.Mode.ModeType;
                NetClientLocate.Log.LogWarning("发送创建房间>>>>", createRoomMsg.gameMode);
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.CreateRoomC2s, createRoomMsg);
            }
            //发送加入房间
            else
            {
                //发送加入
                JoinRoomC2s data = new JoinRoomC2s();
                data.Player = new JoinPlayerInfo();
                data.Player.Name = "zzz";
                data.Player.Id = 1;
                NetClientLocate.Log.LogWarning("发送加入>>>>", "zzz");
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.JoinRoomC2s, data);
            }
        }
    }
}
