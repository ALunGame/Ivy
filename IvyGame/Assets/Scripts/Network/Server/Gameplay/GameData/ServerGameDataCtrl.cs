using Gameplay.Map;
using LiteNetLib;
using Proto;
using System.Collections.Generic;

namespace Game.Network.Server
{
    /// <summary>
    /// 服务器数据层
    /// </summary>
    internal class ServerGameDataCtrl : ServerGameplayProcess
    {
        public int GenUid { get; private set; }

        public List<ServerGamerData> Gamers { get; private set; }
        public ServerMapData Map { get; private set; }

        public override void OnInit()
        {
            GenUid = 0;
            Gamers = new List<ServerGamerData>();
            Map = new ServerMapData();
        }

        public override void OnUpdateLogic(float pDeltaTime, float pGameTime)
        {
            Map.UpdateLogic(pDeltaTime, pGameTime);
            for (int i = 0; i < Gamers.Count; i++)
            {
                Gamers[i].UpdateLogic(pDeltaTime, pGameTime);
            }

            //同步状态
            if (NetServerLocate.GameCtrl.ServerTick % 2 == 0)
            {
                List<GamerBaseState> gamerStates = new List<GamerBaseState>();

                for (int i = 0; i < Gamers.Count; i++)
                {
                    ServerGamerData gamerData = Gamers[i];
                    gamerStates.Add(gamerData.CollectGamerState());
                }

                for (int i = 0; i < Gamers.Count; i++)
                {
                    ServerGamerData gamerData = Gamers[i];

                    ServerStateS2c serverState = new ServerStateS2c();
                    serverState.serverTick = NetServerLocate.GameCtrl.ServerTick;
                    serverState.commandTick = gamerData.LastCommandTick;
                    serverState.gameTime = (int)NetServerLocate.GameCtrl.GameTime;
                    serverState.gamerStates.AddRange(gamerStates);

                    NetServerLocate.Net.SendTo(gamerData.Peer, (ushort)RoomMsgDefine.ServerStateS2c, serverState, DeliveryMethod.Unreliable);
                }
            }
        }

        public override void OnClear()
        {
            Map.Clear();
            Map = null;

            foreach (var item in Gamers)
            {
                item.Clear();
            }
            Gamers.Clear();
        }

        public override void OnEnterMap(int pMapId)
        {
            MapCfg mapCfg = IAConfig.Config.MapCfg[pMapId];
            Map.Create((int)mapCfg.gridSize.x, (int)mapCfg.gridSize.y);
        }

        public override void OnStartGame(int pGameLevelId)
        {
        }

        #region Gamer

        public ServerGamerData AddGamer(NetPeer pPeer, JoinGamerInfo pGamerInfo, string pUidEx = "")
        {
            string uid = $"Gamer_{++GenUid}_{pUidEx}";
            ServerGamerData gamerData = new ServerGamerData(pPeer, uid, pGamerInfo.Id, pGamerInfo.Name);
            Gamers.Add(gamerData);
            return gamerData;
        }

        public void RemoveGamer(string pGamerUid)
        {
            for (int i = 0; i < Gamers.Count; i++)
            {
                if (Gamers[i].GamerUid == pGamerUid)
                {
                    Gamers.RemoveAt(i);
                }
            }
        }

        public ServerGamerData GetGamer(string pGamerUid)
        {
            for (int i = 0; i < Gamers.Count; i++)
            {
                if (Gamers[i].GamerUid == pGamerUid)
                {
                    return Gamers[i];
                }
            }

            return null;
        }


        #endregion
    }
}
