using IAEngine;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    /// <summary>
    /// 单人模式
    /// </summary>
    internal class ServerGameMode_Single : BaseServerGameMode
    {
        public static Vector2Int RebornSize = new Vector2Int(3, 3);

        public override void UpdateLogic(float pDeltaTime, float pGameTime)
        {
            if (pGameTime <= 0) 
            {
                NetServerLocate.GameCtrl.EndGame();
            }
        }

        public override void EnterMap(int pMapId)
        {
            for (int i = 0; i < NetServerLocate.GameCtrl.GameData.Gamers.Count; i++)
            {
                int camp = i + 1;
                ServerGamerData gamerData = NetServerLocate.GameCtrl.GameData.Gamers[i];
                gamerData.SetCamp(camp);
            }
        }

        public override void StartGame(int pGameLvelId)
        {
            //设置阵营和位置
            List<ServerGamerData> gamers = NetServerLocate.GameCtrl.GameData.Gamers;
            ServerMapData map = NetServerLocate.GameCtrl.GameData.Map;
            for (int i = 0; i < gamers.Count; i++)
            {
                ServerGamerData gamer = gamers[i];
                if (map.RandomGetCampRect(RebornSize.x, RebornSize.y, 0, out RectInt outRect))
                {
                    //设置阵营
                    map.SetRectCamp(outRect, gamer.Camp);
                    //设置位置
                    Vector2Int pos = outRect.RandomGetPoint();
                    gamer.SetPos(pos);
                }
                map.AddGamerPathData(gamer);
            }
        }

        public override void OnGamerReborn(ServerGamerData pGamer)
        {
            ServerMapData map = NetServerLocate.GameCtrl.GameData.Map;
            if (map.RandomGetCampRect(RebornSize.x, RebornSize.y, 0, out RectInt outRect))
            {
                //设置阵营
                map.SetRectCamp(outRect, pGamer.Camp);
                //设置位置
                Vector2Int pos = outRect.RandomGetPoint();
                pGamer.SetPos(pos);
            }
            else
            {
                NetServerLocate.Log.LogError("玩家复活失败，没有空余位置",pGamer.GamerUid);
            }
        }
    }
}
