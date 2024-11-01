using GameContext;
using Gameplay;
using IAEngine;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    internal class SevrerGamerSystem : BaseServerGameMapSystem
    {
        public override void OnBeforeEnterMap()
        {
        }

        public override void OnStartGame()
        {
            InitRegGamersData();
        }

        private void InitRegGamersData()
        {
            List<ServerGamerData> gamers = NetServerLocate.GameCtrl.GameData.Gamers;
            for (int i = 0; i < gamers.Count; i++)
            {
                ServerGamerData gamer = gamers[i];
                gamer.GridPos.RegValueChangedEvent((Vector2Int newGridPos) =>
                {
                    OnGamerGridPosChange(gamer, newGridPos);
                });
            }
        }

        private void OnGamerGridPosChange(ServerGamerData pGamer, Vector2Int pGridPos)
        {
            ServerMapData map = NetServerLocate.GameCtrl.GameData.Map;
            map.AddGamerPathPoint(pGamer.GamerUid, pGridPos);

            List<string> dieGamerUids = new List<string>();
            ServerMapGamerPathData gamerPath = map.GetGamerPathData(pGamer.GamerUid);

            //判断有人
            string currGamerUid = map.GetPlayerUidInPathPoint(pGridPos.x, pGridPos.y);
            if (currGamerUid != "" && currGamerUid != pGamer.GamerUid)
            {
                dieGamerUids.Add(currGamerUid);
            }

            //0，判断是不是自己的领地
            int currCamp = map.GetPointCamp(pGridPos.x, pGridPos.y);
            //2，是自己的领地，区域占领
            if (currCamp == pGamer.Camp)
            {
                gamerPath.CaptureArea((pPos) =>
                {
                    ServerGamerData dieGamer = map.GetGamer(pPos);
                    if (dieGamer != null && dieGamer.GamerUid != pGamer.GamerUid)
                    {
                        if (!dieGamerUids.Contains(dieGamer.GamerUid))
                        {
                            dieGamerUids.Add(dieGamer.GamerUid);
                        }
                    }
                });
                //自己路径清理
                gamerPath.Clear();
            }
            //3，不是自己的领地，处理路径连接问题
            else
            {
                //路径连接了，清空路径
                if (currGamerUid == pGamer.GamerUid)
                {
                    gamerPath.RemovePath(pGamer.LastGridPos, pGamer.GridPos.Value);
                }
                else
                {
                    gamerPath.AddPoint(pGridPos);
                }
            }

            //4，处理击杀的玩家
            if (dieGamerUids.IsLegal())
            {
                NetServerLocate.GameCtrl.GameMode.GamerDie(dieGamerUids, pGamer.GamerUid);
            }
        }
    }
}
