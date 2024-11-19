using Game.Helper;
using Game.Network;
using Gameplay.GameData;
using Gameplay.GameMap;
using Gameplay.GameMode;
using Gameplay.Map;
using IAToolkit;
using IAUI;
using Proto;
using System;
using UnityEngine;


namespace Gameplay
{
    public enum GameModeType : byte
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,

        /// <summary>
        /// 单人
        /// </summary>
        Single,

        /// <summary>
        /// 团队
        /// </summary>
        Team,
    }

    public class GameplayCtrl : MonoSingleton<GameplayCtrl>
    {
        public int CurrGameLevel { get; private set; }

        public int CurrMapId { get; private set; }

        public GameModeType GameModeType { get; private set; }

        /// <summary>
        /// 游戏状态
        /// </summary>
        public GameState State { get; private set; }

        public float TimeScale { get; set; }

        [ReadOnly]
        public float GameTime;

        public float GameDeltaTime { get; set; }

        public int LastServerTick { get; set; }

        public GameMapCtrl GameMap { get; private set; }

        public GameModeCtrl GameMode { get; private set; }

        public GameDataCtrl GameData { get; private set; }

        private void Update()
        {
            UpdateLogic();
        }

        private void OnApplicationQuit()
        {
            ClearLogic();
        }

        #region 逻辑生命周期

        private void InitLogic()
        {
            State = GameState.None;

            TimeScale = 1.0f;
            GameTime = 0.0f;
            GameDeltaTime = Time.deltaTime;

            GameMode = new GameModeCtrl();
            GameMode.Init(this);

            GameData = new GameDataCtrl();
            GameData.Init(this);

            GameMap = new GameMapCtrl();
            GameMap.Init(this);
        }

        private void UpdateLogic()
        {
            if (State != GameState.Start)
            {
                return;
            }

            float deltaTime = GameDeltaTime * TimeScale;
            GameTime += deltaTime;
            GameMode.UpdateLogic(deltaTime, GameTime);
            GameData.UpdateLogic(deltaTime, GameTime);
            GameMap.UpdateLogic(deltaTime, GameTime);
        }

        private void ClearLogic()
        {
            GameMode.Clear();
            GameData.Clear();
            GameMap.Clear();
        }

        #endregion

        #region 游戏生命周期

        /// <summary>
        /// 创建游戏
        /// </summary>
        /// <param name="pModeType">游戏模式</param>
        /// <param name="pNeedCreateRoom">是否创建房间，房主</param>
        public void CreateGame(GameModeType pModeType, bool pNeedCreateRoom)
        {
            InitLogic();

            GameModeType = pModeType;

            GameMode.OnCreateGame(pModeType, pNeedCreateRoom);
            GameData.OnCreateGame(pModeType, pNeedCreateRoom);
            GameMap.OnCreateGame(pModeType, pNeedCreateRoom);

            State = GameState.Wait;
        }

        /// <summary>
        /// 进入地图
        /// </summary>
        /// <param name="pMapId">地图Id</param>
        public void EnterMap(int pMapId)
        {
            CurrMapId = pMapId;

            GameMode.OnEnterMap(CurrMapId);

            GameData.OnEnterMap(CurrMapId);

            GameMap.OnEnterMap(CurrMapId);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="pGameLevelId">关卡Id</param>
        public void StartGame(int pGameLevelId)
        {
            if (!IAConfig.Config.GameLevelCfg.ContainsKey(pGameLevelId))
            {
                Debug.LogError("开始游戏失败，没有对应的关卡:" + pGameLevelId);
                return;
            }

            GameLevelCfg cfg = IAConfig.Config.GameLevelCfg[pGameLevelId];

            CurrGameLevel = pGameLevelId;
            CurrMapId = cfg.mapId;

            GameMode.OnStartGame(pGameLevelId);
            GameData.OnStartGame(pGameLevelId);
            GameMap.OnStartGame(pGameLevelId);

            State = GameState.Start;
        }

        public void PauseGame()
        {
            TimeScale = 0;

            GameMode.OnPauseGame();
            GameData.OnPauseGame();
            GameMap.OnPauseGame();
        }

        public void ReuseGame()
        {
            TimeScale = 1;

            GameMode.OnReuseGame();
            GameData.OnReuseGame();
            GameMap.OnReuseGame();
        }

        public void EndGame()
        {
            GameMode.OnEndGame();
            GameData.OnEndGame();
            GameMap.OnEndGame();

            State = GameState.End;

            UILocate.UI.Show(UIPanelDef.GameEndPanel);
        }

        public void ExitGame()
        {
            GameMode.OnExitGame();
            GameData.OnExitGame();
            GameMap.OnExitGame();

            ClearLogic();
            GameObject.Destroy(this);
        }

        #endregion

        private void ResetTime()
        {
            TimeScale = 1.0f;
            GameTime = 0.0f;
            GameDeltaTime = UnityEngine.Time.deltaTime;
        }

        public void OnReceiveServerState(ServerStateS2c pMsgData)
        {
            if (NetworkGeneral.SeqDiff(pMsgData.serverTick, LastServerTick) <= 0)
                return;

            GameTime = pMsgData.gameTime;
            LastServerTick = pMsgData.serverTick;
            GameData.OnReceiveServerState(pMsgData);
        }
    }
}
