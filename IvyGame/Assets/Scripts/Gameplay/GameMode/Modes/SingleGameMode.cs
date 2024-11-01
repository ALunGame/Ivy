using Game.Network;
using Game.Network.Client;
using Game.Network.Server;
using Gameplay.GameData;
using Gameplay.GameMap.Actor;
using Gameplay.GameMap.System;
using Proto;
using UnityEngine;

namespace Gameplay.GameMode
{
    /// <summary>
    /// 单人模式
    /// </summary>
    public class SingleGameMode : BaseGameMode
    {
        private NetServer Server;

        private NetClient Client;

        private bool openDiscovery = false;         //开启广播用于发现服务器
        private float discoveryInterval = 1.0f;     //广播间隔时间
        private float lastDiscoveryTime = 0.0f;

        public override void OnInit()
        {
            //创建房间服务器
            if (CreateRoom)
            {
                Server = GameModeGo.AddComponent<NetServer>();
                Server.StartServer();
            }

            //客户端
            openDiscovery = true;
            Client = GameModeGo.AddComponent<NetClient>();
            Client.SetDiscoveryCallBack((endPoint) =>
            {
                openDiscovery = false;
                Client.Connect(endPoint, OnConnectServer, null);
            });
            SendDiscovery();
        }

        public override void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (openDiscovery)
            {
                if (pGameTime - lastDiscoveryTime > discoveryInterval)
                {
                    SendDiscovery();
                    lastDiscoveryTime = pGameTime;
                }
            }
        }

        public override void OnClear()
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

        #region 寻找服务器

        private void SendDiscovery()
        {
            NetClientLocate.Log.Log("寻找服务器...", NetworkGeneral.ServerPort);
            Client.Discovery(NetworkGeneral.ServerPort);
        }

        private void OnConnectServer()
        {
            //发送创建房间
            if (CreateRoom)
            {
                CreateRoomC2s createRoomMsg = new CreateRoomC2s();
                createRoomMsg.gameMode = (int)GameModeType.Single;
                createRoomMsg.Gamer = new JoinGamerInfo();
                createRoomMsg.Gamer.Id = 101;
                createRoomMsg.Gamer.Name = "alun";
                createRoomMsg.Gamer.fightMusicId = 1;

                NetClientLocate.Log.LogWarning("发送创建房间>>>>", createRoomMsg.gameMode);
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.CreateRoomC2s, createRoomMsg);
            }
            //发送加入房间
            else
            {
                //发送加入
                JoinRoomC2s data = new JoinRoomC2s();
                data.Gamer = new JoinGamerInfo();
                data.Gamer.Name = "zzz";
                data.Gamer.Id = 101;
                data.Gamer.fightMusicId = 1;
                NetClientLocate.Log.LogWarning("发送加入>>>>", "zzz");
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.JoinRoomC2s, data);
            }
        }

        #endregion

        #region 游戏流程

        public override void StartGame(int pGameLevelId)
        {
            //foreach (GamerData gamerData in GameplayGlobal.Data.Gamers.Gamers)
            //{
            //    GameMapGridData gridData = GameplayGlobal.Data.Map.GetGridData(gamerData.GridPos.Value);
            //    gridData.Camp.Value = gamerData.Camp;
            //}
        }

        public override void EndGame()
        {
            
        }

        #endregion
    }
}
