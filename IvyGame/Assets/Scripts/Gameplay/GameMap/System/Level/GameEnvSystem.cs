using Game.Network.Client;
using IAFramework;
using Proto;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.System
{
    /// <summary>
    /// 负责处理关卡场景逻辑
    /// </summary>
    public class GameEnvSystem : BaseGameMapSystem
    {
        private MapGrids mapGrids;

        public override void OnUpdate(float pDeltaTime, float pGameTime)
        {
            if (mapGrids)
            {
                mapGrids.UpdateLogic(pDeltaTime, pGameTime);
            }
        }

        public override void OnEnterMap()
        {
            CreateMapGrids();
            NetworkEvent.RegisterEvent("GameEnvSystem", (ushort)RoomMsgDefine.ChangeGridCampS2c, OnGridsCampChange);
        }

        public override void OnExitMap()
        {
            if (mapGrids)
            {
                GameObject.Destroy(mapGrids.gameObject);
                mapGrids = null;
            }
            NetworkEvent.RemoveEvent("GameEnvSystem");
        }

        private void CreateMapGrids()
        {
            GameObject gridsGo = GameEnv.Asset.CreateGo("GameMap_Grids");
            gridsGo.transform.SetParent(GameplayGlobal.Map.MapTrans);
            gridsGo.name = "GameMap_Grids";

            mapGrids = gridsGo.GetComponent<MapGrids>();
            mapGrids.CreateMap(GameplayGlobal.Data.Map.MapSize);

            //GameActorSystem actorSystem = GameplayCtrl.Instance.GameMap.GetSystem<GameActorSystem>();
            //MapCfg mapCfg = Config.MapCfg[GameplayCtrl.Instance.CurrMapId];

            //foreach (int posX in GameplayGlobal.Data.Map.MapGrid.Keys)
            //{
            //    foreach (int posY in GameplayGlobal.Data.Map.MapGrid[posX].Keys)
            //    {
            //        GameMapGridData gridData = GameplayGlobal.Data.Map.MapGrid[posX][posY];
            //        gridData.Camp.RegValueChangedEvent((pCamp) =>
            //        {
            //            //mapGrids.ChangeGridCamp(new Vector2Int(posX, posY), pCamp);
            //        });
            //    }
            //}
        }

        private void OnGridsCampChange(IExtensible pMsg)
        {
            ChangeGridCampS2c msg = (ChangeGridCampS2c)pMsg;
            List<Vector2Int> posList = new List<Vector2Int>();
            foreach (var gridPos in msg.gridPosLists)
            {
                posList.Add(new Vector2Int((int)gridPos.X, (int)gridPos.Y));
            }
            mapGrids.ChangeGridsCamp(posList, msg.Camp);
        }
    }
}
