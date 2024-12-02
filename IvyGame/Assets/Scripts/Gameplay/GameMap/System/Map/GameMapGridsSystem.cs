using Game.Network.Client;
using Gameplay.GameMap.Actor;
using IAEngine;
using IAFramework;
using Proto;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.System
{
    /// <summary>
    /// 地图格子系统
    /// </summary>
    internal class GameMapGridsSystem : BaseGameMapSystem
    {
        private List<Actor_InternalGamer> allGamers;

        public bool UseGPU = false;
        public MapGridsGPU MapGridsGPU { get; private set; }
        public MapGridsCPU MapGridsCPU { get; private set; }

        public override void OnEnterMap()
        {
            CreateMapGrids();
            NetworkEvent.RegisterEvent(GetType().Name, (ushort)RoomMsgDefine.ChangeGridCampS2c, OnGridsCampChange);
        }

        public override void OnStartGame()
        {
            allGamers = GameplayGlobal.Map.GetSystem<GameActorSystem>().GetActors<Actor_InternalGamer>();
            if (allGamers.IsLegal())
            {
                for (int i = 0; i < allGamers.Count; i++)
                {
                    Actor_InternalGamer gamer = allGamers[i];
                    gamer.GridPos.RegValueChangedEvent((gridPos) =>
                    {
                        MapGridsGPU?.OnGamerThroughCampArea(gamer.Camp, gridPos);
                        MapGridsCPU?.OnGamerThroughCampArea(gamer.Camp, gridPos);
                    });

                    gamer.GamerData.OnRemovePathPoint += (pos) =>
                    {
                        MapGridsGPU?.OnGamerPathChange(new List<Vector2Int>() { pos }, 2);
                        MapGridsCPU?.OnGamerPathChange(new List<Vector2Int>() { pos }, 2);
                    };
                    gamer.GamerData.OnClearPath += (poslist) =>
                    {
                        MapGridsGPU?.OnGamerPathChange(poslist, 3);
                        MapGridsCPU?.OnGamerPathChange(poslist, 3);
                    };
                }
            }
        }

        public override void OnUpdate(float pDeltaTime, float pGameTime)
        {
            if (MapGridsGPU)
            {
                MapGridsGPU.UpdateLogic(pDeltaTime, pGameTime);
            }

            if (MapGridsCPU)
            {
                MapGridsCPU.UpdateLogic(pDeltaTime, pGameTime);
            }
        }

        public override void OnExitMap()
        {
            if (MapGridsGPU)
            {
                GameObject.Destroy(MapGridsGPU.gameObject);
                MapGridsGPU = null;
            }

            if (MapGridsCPU)
            {
                GameObject.Destroy(MapGridsCPU.gameObject);
                MapGridsCPU = null;
            }
            NetworkEvent.RemoveEvent(GetType().Name);
        }

        private void CreateMapGrids()
        {
            if (UseGPU)
            {
                GameObject gridsGo = GameEnv.Asset.CreateGo("GameMap_GridsGPU");
                gridsGo.transform.SetParent(GameplayGlobal.Map.MapTrans);
                gridsGo.name = "GameMap_Grids";

                MapGridsGPU = gridsGo.GetComponent<MapGridsGPU>();
                MapGridsGPU.CreateMap(GameplayGlobal.Data.Map.MapSize);
            }
            else
            {
                GameObject gridsGo = GameEnv.Asset.CreateGo("GameMap_GridsCPU");
                gridsGo.transform.SetParent(GameplayGlobal.Map.MapTrans);
                gridsGo.name = "GameMap_Grids";

                MapGridsCPU = gridsGo.GetComponent<MapGridsCPU>();
                //MapGridsCPU.CreateMap(GameplayGlobal.Data.Map.MapSize);
            }

        }

        private void OnGridsCampChange(IExtensible pMsg)
        {
            ChangeGridCampS2c msg = (ChangeGridCampS2c)pMsg;
            List<Vector2Int> posList = new List<Vector2Int>();
            foreach (var gridPos in msg.gridPosLists)
            {
                posList.Add(new Vector2Int((int)gridPos.X, (int)gridPos.Y));
            }
            MapGridsGPU?.ChangeGridsCamp(posList, msg.Camp);
            MapGridsCPU?.ChangeGridsCamp(posList, msg.Camp);
        }
    }
}
