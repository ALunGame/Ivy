using Gameplay.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameData
{
    public class GameMapGridData : BaseGameData
    {
        public Vector2Int GridPos {  get; private set; }

        public GameDataField<int> Camp {  get; private set; }

        public GameMapGridData(Vector2Int pGridPos)
        {
            GridPos = pGridPos;
            Camp = new GameDataField<int>(this);
        }
    }

    public class GameMapData : BaseGameData
    {
        public int MapId { get; private set; }

        public Vector2Int MapSize { get; private set; }

        private Dictionary<int, Dictionary<int, GameMapGridData>> mapGrid = new Dictionary<int, Dictionary<int, GameMapGridData>>();
        public Dictionary<int, Dictionary<int, GameMapGridData>> MapGrid
        {
            get
            {
                return mapGrid;
            }
        }

        public Dictionary<int, List<GameMapGridData>> campGrids = new Dictionary<int, List<GameMapGridData>>();
        public Dictionary<int, List<GameMapGridData>> CampGrids
        {
            get
            {
                return campGrids;
            }
        }

        public void OnEnterMap(int pMapId)
        {
            MapCfg mapCfg = IAConfig.Config.MapCfg[pMapId];
            MapId = pMapId;
            MapSize = new Vector2Int((int)mapCfg.gridSize.x, (int)mapCfg.gridSize.y);

            CreateMapGrids(mapCfg);

            Debug.LogError("GameMapData::OnEnterMap-->>>");
        }


        public void OnStartGame(int pGameLvelId)
        {
            GameLevelCfg cfg = IAConfig.Config.GameLevelCfg[pGameLvelId];
            Debug.LogError("GameMapData::OnStartGame-->>>");
        }

        public override void OnClear()
        {
            mapGrid.Clear();
        }

        private void CreateMapGrids(MapCfg pMapCfg)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    GameMapGridData gridData = new GameMapGridData(new Vector2Int(x, y));
                    gridData.Camp.SetValueWithoutNotify(0);
                    gridData.Camp.RegValueChangedEvent((camp, oldCamp) =>
                    {
                        //清空旧的
                        if (campGrids.ContainsKey(oldCamp))
                        {
                            if (campGrids[oldCamp].Contains(gridData))
                            {
                                campGrids[oldCamp].Remove(gridData);
                            }
                        }
                           
                        //保存新的
                        if (!campGrids.ContainsKey(camp))
                            campGrids.Add(camp, new List<GameMapGridData>());
                        campGrids[camp].Add(gridData);
                    });

                    if (!mapGrid.ContainsKey(x))
                        mapGrid.Add(x, new Dictionary<int, GameMapGridData>());
                    mapGrid[x].Add(y, gridData);
                }
            }
        }

        /// <summary>
        /// 检测坐标合法
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public bool CheckPosLegal(Vector2 pPos)
        {
            int posX = (int)pPos.x;
            if (posX < 0 || posX >= MapSize.x)
            {
                return false;
            }

            int posY = (int)pPos.y;
            if (posY < 0 || posY >= MapSize.y)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 坐标转格子
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public Vector2Int PosToGrid(Vector2 pPos)
        {
            return new Vector2Int((int)pPos.x, (int)pPos.y);
        }

        /// <summary>
        /// 获取格子数据
        /// </summary>
        /// <param name="pGridPos"></param>
        /// <returns></returns>
        public GameMapGridData GetGridData(Vector2Int pGridPos)
        {
            if (!mapGrid.ContainsKey(pGridPos.x) || !mapGrid[pGridPos.x].ContainsKey(pGridPos.y))
            {
                Debug.LogError($"GetGridData错误，没有对应格子:{pGridPos}");
                return null;
            }
            return mapGrid[pGridPos.x][pGridPos.y];
        }
    }
}
