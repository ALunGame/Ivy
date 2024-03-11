using Game;
using Game.Network.Client;
using Game.Network.Server;
using Gameplay.Grid;
using Gameplay.Player;
using Gameplay.System;
using IAFramework;
using Proto;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// 游戏实例
    /// 1，单例，只能存在一个游戏实例
    /// 2，包含对GameMode的引用
    /// 3，Gameplay的生命周期
    /// </summary>
    internal class GameInstance : MonoBehaviour
    {
        /// <summary>
        /// 游戏模式
        /// </summary>
        public GameMode Mode {  get; private set; }

        /// <summary>
        /// 是否是房主
        /// </summary>
        public bool IsRoomOwner { get; private set; }

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsInit {  get; private set; }

        /// <summary>
        /// 当前游戏时间
        /// </summary>
        public float CurrGameTime { get; private set; }

        #region UpdateProps
        /// <summary>
        /// 帧已经运行秒数
        /// </summary>
        public float UpdateRealElapseSeconds { get; private set; }
        /// <summary>
        /// 每帧间隔时间
        /// </summary>
        public float UpdateDeltaTime { get; private set; }
        /// <summary>
        /// 每帧时间缩放
        /// </summary>
        public float UpdateTimeScale { get; private set; }

        /// <summary>
        /// 固定频率已经运行秒数
        /// </summary>
        public float FixedUpdateRealElapseSeconds { get; private set; }
        /// <summary>
        /// 固定频率间隔时间
        /// </summary>
        public float FixedUpdateDeltaTime { get; private set; }
        /// <summary>
        /// 固定频率时间缩放
        /// </summary>
        public float FixedUpdateTimeScale { get; private set; } 
        #endregion

        //玩家
        public List<GamePlayer> Players { get; private set; }

        //地图格子
        public GameGrid[,] MapGrids { get; private set; }

        //玩家路径
        public Dictionary<int,List<GamePlayerPath>> PlayerPath {  get; private set; }

        #region Transforms

        //地图根节点
        public Transform GameMapRootTrans { get; private set; }
        //地图节点
        public Transform GameMapTrans { get; private set; }
        //地图格子根节点
        public Transform GameMapGridRootTrans { get; private set; }
        //玩家根节点
        public Transform PlayerRootTrans { get; private set; }
        //玩家路径根节点
        public Transform PlayerPathRootTrans { get; private set; }

        #endregion

        //玩法系统
        private List<GameplaySystem> systems = new List<GameplaySystem>()
        {
            new NetworkSystem(),
            new SyncSystem(),
            new PlayerSystem(),

            new InputSystem(),

            new CameraSystem(),
        };

        #region Unity

        private void Awake()
        {
            Players = new List<GamePlayer>();
            PlayerPath = new Dictionary<int, List<GamePlayerPath>>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            UpdateRealElapseSeconds += UpdateDeltaTime * UpdateTimeScale;
            CurrGameTime -= UpdateDeltaTime * UpdateTimeScale;
            foreach (GameplaySystem system in systems)
                system.Update(UpdateDeltaTime * UpdateTimeScale, UpdateRealElapseSeconds);
        }

        private void FixedUpdate()
        {
            FixedUpdateRealElapseSeconds += FixedUpdateDeltaTime * FixedUpdateTimeScale;
            foreach (GameplaySystem system in systems)
                system.FixedUpdate(FixedUpdateDeltaTime * FixedUpdateTimeScale, UpdateRealElapseSeconds);
        }

        private void OnDestroy()
        {
            foreach (GameplaySystem system in systems)
                system.Clear();
        }

        #endregion

        #region 生命周期

        public void Init(GameModeType modeType, bool isRoomOwner)
        {
            if (IsInit)
            {
                return;
            }
            IsInit = true;
            IsRoomOwner = isRoomOwner;

            GameMapRootTrans = transform;

            Mode = new GameMode();
            Mode.ModeType = modeType;

            UpdateRealElapseSeconds = 0;
            UpdateDeltaTime = Time.deltaTime;
            UpdateTimeScale = Time.timeScale;

            FixedUpdateRealElapseSeconds = 0;
            FixedUpdateDeltaTime = Time.fixedDeltaTime;
            FixedUpdateTimeScale = Time.timeScale;

            GameplayLocate.SetGameInstance(this);

            foreach (GameplaySystem system in systems)
                system.Init();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            CurrGameTime = TempConfig.GameTotalTime;

            CreateMap();

            CreateMapGird();

            foreach (GameplaySystem system in systems)
                system.StartGame();

            foreach (GameplaySystem system in systems)
                system.AfterStartGame();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            CurrGameTime = 0;

            Destroy(GameMapTrans.gameObject);

            foreach (GameplaySystem system in systems)
                system.EndGame();
        }

        private void CreateMap()
        {
            string prefabName = "GameMap_Single";
            GameObject mapGo = Instantiate(GameEnv.Asset.LoadPrefab(prefabName));
            mapGo.transform.SetParent(GameMapRootTrans);
            mapGo.transform.localPosition = Vector3.zero;   

            GameMapTrans = mapGo.transform;
            GameMapGridRootTrans = GameMapTrans.Find("GridRoot");
            PlayerRootTrans = GameMapTrans.Find("PlayerRoot");
            PlayerPathRootTrans = GameMapTrans.Find("PlayerPathRoot");
        }

        #endregion

        #region 玩家

        public void AddPlayer(AddGamePlayerInfo addInfo)
        {
            RemovePlayer(addInfo.uid);

            string prefabName = "Player";
            GameObject playerGo = Instantiate(GameEnv.Asset.LoadPrefab(prefabName));
            playerGo.name = $"{addInfo.name}->{addInfo.uid}";
            playerGo.transform.SetParent(PlayerRootTrans);

            GamePlayer player = playerGo.AddComponent<GamePlayer>();
            player.Init(addInfo);

            Players.Add(player);
        }

        public GamePlayer GetPlayer(int playerUid)
        {
            foreach (GamePlayer player in Players)
            {
                if (player.Uid == playerUid)
                {
                    return player;
                }
            }
            return null;
        }

        public void RemovePlayer(int playerUid)
        {
            GamePlayer player = GetPlayer(playerUid);
            if (player != null)
            {
                Players.Remove(player);
            }
        }

        #endregion

        #region 玩家路径

        public void AddPlayerPath(int playerUid, byte posX, byte posY)
        {
            if (!PlayerPath.ContainsKey(playerUid))
            {
                PlayerPath.Add(playerUid, new List<GamePlayerPath>());
            }

            string prefabName = "PlayerPath";
            GameObject pathGo = Instantiate(GameEnv.Asset.LoadPrefab(prefabName));
            pathGo.name = $"(x={posX},y={posY})";
            pathGo.transform.SetParent(PlayerPathRootTrans);

            GamePlayerPath pathCom = pathGo.AddComponent<GamePlayerPath>();
            pathCom.Init(GetPlayer(playerUid), posX, posY);
            PlayerPath[playerUid].Add(pathCom);
        }

        public void RemovePlayerPath(int playerUid, byte posX, byte posY)
        {
            if (!PlayerPath.ContainsKey(playerUid))
            {
                return;
            }

            List<GamePlayerPath> paths = PlayerPath[playerUid];
            for (int i = 0; i < paths.Count; i++)
            {
                GamePlayerPath pathCom = paths[i];
                if (pathCom.PosX == posX && pathCom.PosY == posY)
                {
                    Destroy(pathCom.gameObject);
                    paths.RemoveAt(i);
                }
            }
        }

        public void ClearPlayerPath(int playerUid)
        {
            if (!PlayerPath.ContainsKey(playerUid))
            {
                return;
            }

            List<GamePlayerPath> paths = PlayerPath[playerUid];
            for (int i = 0; i < paths.Count; i++)
            {
                GamePlayerPath pathCom = paths[i];
                Destroy(pathCom.gameObject);
            }
            PlayerPath.Remove(playerUid);
        }

        #endregion

        #region 地图格子

        private void CreateMapGird()
        {
            string prefabName = "MapGrid";
            MapGrids = new GameGrid[TempConfig.MapSize.x, TempConfig.MapSize.y];

            for (byte x = 0; x < TempConfig.MapSize.x; x++)
            {
                for (byte y = 0; y < TempConfig.MapSize.y; y++)
                {
                    GameObject gridGo = Instantiate(GameEnv.Asset.LoadPrefab(prefabName));
                    gridGo.name = $"(x={x},y={y})";
                    gridGo.transform.SetParent(GameMapGridRootTrans);

                    GameGrid gameGrid = gridGo.AddComponent<GameGrid>();
                    gameGrid.Init(x, y);
                    MapGrids[x,y] = gameGrid;

                }
            }
        }

        public void ChanegMagGridCamp(byte posX, byte posY, byte camp)
        {
            MapGrids[posX, posY].ChangeCamp(camp);
        }

        #endregion

        /// <summary>
        /// 服务器坐标转客户端坐标
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static Vector3 ServerPosToClient(byte posX, byte posY)
        {
            //return new Vector3(TempConfig.GridSize.x / 2 + posX, 0, TempConfig.GridSize.y / 2 + posY);
            return new Vector3(posX, 0, posY);
        }
    }
}
