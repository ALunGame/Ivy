using Gameplay.GameData;
using Gameplay.GameMap.Actor;
using Gameplay.Map;
using IAConfig;
using IAFramework;
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
        }

        public override void OnExitMap()
        {
            if (mapGrids)
            {
                GameObject.Destroy(mapGrids.gameObject);
                mapGrids = null;
            }
        }

        private void CreateMapGrids()
        {
            GameObject gridsGo = GameEnv.Asset.CreateGo("GameMap_Grids");
            gridsGo.transform.SetParent(GameplayGlobal.Map.MapTrans);
            gridsGo.name = "GameMap_Grids";

            mapGrids = gridsGo.GetComponent<MapGrids>();
            mapGrids.CreateMap(GameplayGlobal.Data.Map.MapSize);

            GameActorSystem actorSystem = GameplayCtrl.Instance.GameMap.GetSystem<GameActorSystem>();
            MapCfg mapCfg = Config.MapCfg[GameplayCtrl.Instance.CurrMapId];

            foreach (int posX in GameplayGlobal.Data.Map.MapGrid.Keys)
            {
                foreach (int posY in GameplayGlobal.Data.Map.MapGrid[posX].Keys)
                {
                    GameMapGridData gridData = GameplayGlobal.Data.Map.MapGrid[posX][posY];
                    gridData.Camp.RegValueChangedEvent((pCamp) =>
                    {
                        mapGrids.ChangeGridCamp(new Vector2Int(posX, posY), pCamp);
                    });
                }
            }
        }
    }
}
