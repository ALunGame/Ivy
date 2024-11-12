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
        public MapGrids MapGrids { get; private set; }

        public override void OnEnterMap()
        {
            CreateMapGrids();
            NetworkEvent.RegisterEvent(GetType().Name, (ushort)RoomMsgDefine.ChangeGridCampS2c, OnGridsCampChange);
        }

        public override void OnStartGame()
        {
            allGamers = GameplayGlobal.Map.GetSystem<GameActorSystem>().GetActors<Actor_InternalGamer>();
        }

        public override void OnUpdate(float pDeltaTime, float pGameTime)
        {
            if (MapGrids)
            {
                MapGrids.UpdateLogic(pDeltaTime, pGameTime);
                if (allGamers.IsLegal())
                {
                    for (int i = 0; i < allGamers.Count; i++)
                    {
                        //if (allGamers[i].Speed > 0)
                        //{

                        //}
                        MapGrids.PassCampRect(allGamers[i].Uid, allGamers[i].GetGridPos());
                    }
                }
            }
        }

        public override void OnExitMap()
        {
            if (MapGrids)
            {
                GameObject.Destroy(MapGrids.gameObject);
                MapGrids = null;
            }
            NetworkEvent.RemoveEvent(GetType().Name);
        }

        private void CreateMapGrids()
        {
            GameObject gridsGo = GameEnv.Asset.CreateGo("GameMap_Grids");
            gridsGo.transform.SetParent(GameplayGlobal.Map.MapTrans);
            gridsGo.name = "GameMap_Grids";

            MapGrids = gridsGo.GetComponent<MapGrids>();
            MapGrids.CreateMap(GameplayGlobal.Data.Map.MapSize);
        }

        private void OnGridsCampChange(IExtensible pMsg)
        {
            ChangeGridCampS2c msg = (ChangeGridCampS2c)pMsg;
            List<Vector2Int> posList = new List<Vector2Int>();
            foreach (var gridPos in msg.gridPosLists)
            {
                posList.Add(new Vector2Int((int)gridPos.X, (int)gridPos.Y));
            }
            MapGrids.ChangeGridsCamp(posList, msg.Camp);
        }
    }
}
