using Gameplay.GameMap.Actor;
using Gameplay.Map;
using IAConfig;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.System
{
    /// <summary>
    /// 负责处理关卡场景逻辑
    /// </summary>
    public class GameEnvSystem : BaseGameMapSystem
    {
        private Dictionary<int, Dictionary<int, Actor_Grid>> mapGridDict = new Dictionary<int, Dictionary<int, Actor_Grid>>();
        public Dictionary<int, Dictionary<int, Actor_Grid>> MapGridDict { 
            get 
            {
                return mapGridDict;
            } 
        }

        public override void OnEnterMap()
        {
            CreateMapGrids();
        }

        public override void OnExitMap()
        {
            mapGridDict.Clear();
        }

        private void CreateMapGrids()
        {
            GameActorSystem actorSystem = GameplayCtrl.Instance.GameMap.GetSystem<GameActorSystem>();
            MapCfg mapCfg = Config.MapCfg[GameplayCtrl.Instance.CurrMapId];

            foreach (int posX in GameplayGlobal.Data.Map.MapGrid.Keys)
            {
                foreach (int posY in GameplayGlobal.Data.Map.MapGrid[posX].Keys)
                {
                    Actor_Grid actor = actorSystem.CreateActor($"Grid_({posX},{posY})", 201, ActorType.MapGrid, "Grids") as Actor_Grid;
                    actor.SetPos(new Vector2(posX, posY));
                    actor.SetData(GameplayGlobal.Data.Map.MapGrid[posX][posY]);
                }
            }
        }
    }
}
