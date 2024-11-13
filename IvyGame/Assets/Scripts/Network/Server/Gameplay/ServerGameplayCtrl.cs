using Gameplay;
using Gameplay.Map;
using LiteNetLib;
using Proto;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    /// <summary>
    /// 服务器端玩法控制
    /// </summary>
    internal class ServerGameplayCtrl
    {
        public int CurrGameLevel { get; private set; }

        public int CurrMapId { get; private set; }

        /// <summary>
        /// 游戏玩法类型
        /// </summary>
        public GameModeType ModeType { get; private set; }

        /// <summary>
        /// 游戏状态
        /// </summary>
        public GameState State { get; private set; }

        public float TimeScale { get; set; }

        public float GameTime { get; private set; }

        public float GameDeltaTime { get; private set; }

        public ushort ServerTick { get; private set; }

        public ServerGameModeCtrl GameMode { get; private set; }

        public ServerGameDataCtrl GameData { get; private set; }

        public SevrerGameMapCtrl GameMap { get; private set; }

        public void Init()
        {
            State = GameState.None;

            TimeScale = 1.0f;
            GameTime = 0.0f;
            GameDeltaTime = NetworkLogicTimer.FixedDelta;

            InitLogic();
        }

        public void Clear()
        {
            ClearLogic();
        }

        #region 逻辑生命周期

        public void InitLogic()
        {
            GameMode = new ServerGameModeCtrl();
            GameMode.Init(this);

            GameData = new ServerGameDataCtrl();
            GameData.Init(this);

            GameMap = new SevrerGameMapCtrl();
            GameMap.Init(this);
        }

        public void UpdateLogic()
        {
            if (State != GameState.Start)
            {
                return;
            }

            ServerTick = (ushort)((ServerTick + 1) % NetworkGeneral.MaxGameSequence);

            float deltaTime = GameDeltaTime * TimeScale;
            GameTime -= deltaTime;
            if (GameTime < 0.0f)
            {
                GameTime = 0.0f;
            }

            GameMode.UpdateLogic(deltaTime, GameTime);
            GameData.UpdateLogic(deltaTime, GameTime);  
            GameMap.UpdateLogic(deltaTime, GameTime);
        }

        public void ClearLogic()
        {
            GameMode.Clear();
            GameData.Clear();
            GameMap.Clear();
        } 

        #endregion

        private void ResetTime()
        {
            TimeScale = 1.0f;
            GameTime = 0.0f;
            GameDeltaTime = NetworkLogicTimer.FixedDelta;
        }

        #region 游戏生命周期

        /// <summary>
        /// 创建游戏
        /// </summary>
        /// <param name="pPeer"></param>
        /// <param name="pMsgData"></param>
        public void CreateGame(NetPeer pPeer, CreateRoomC2s pMsgData)
        {
            //设置游戏类型
            ModeType = (GameModeType)pMsgData.gameMode;

            //添加房主
            ServerGamerData roomMaster = GameData.AddGamer(pPeer, pMsgData.Gamer);

            //各个模块初始化
            GameMode.OnCreateGame(ModeType, pPeer, roomMaster.GamerUid, pMsgData);
            GameData.OnCreateGame(ModeType, pPeer, roomMaster.GamerUid, pMsgData);
            GameMap.OnCreateGame(ModeType, pPeer, roomMaster.GamerUid, pMsgData);

            //发送创建房间成功
            CreateRoomS2c createMsg = new CreateRoomS2c();
            createMsg.RetCode = 0;
            createMsg.gameMode = pMsgData.gameMode;
            NetServerLocate.Net.SendTo(pPeer, (ushort)RoomMsgDefine.CreateRoomS2c, createMsg);

            //发送房主加入房间成功
            JoinRoomS2c joinMsg = new JoinRoomS2c();
            joinMsg.RetCode = 0;
            joinMsg.gameMode = (int)ModeType;
            joinMsg.roomMastergamerUid = roomMaster.GamerUid;
            joinMsg.selfgamerUid = roomMaster.GamerUid;
            NetServerLocate.Net.SendTo(pPeer, (ushort)RoomMsgDefine.JoinRoomS2c, joinMsg);

            //广播其他人有人加入
            RoomMembersChangeS2c changeMsg = new RoomMembersChangeS2c();
            foreach (ServerGamerData item in GameData.Gamers)
            {
                changeMsg.Gamers.Add(item.CollectGamerInfo());
            }
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.RoomMembersChangeS2c, changeMsg);

            State = GameState.Wait;
        }

        /// <summary>
        /// 进入地图
        /// </summary>
        /// <param name="pMapId"></param>
        public void EnterMap(int pMapId)
        {
            CurrMapId = pMapId;

            GameMode.OnEnterMap(pMapId);
            GameData.OnEnterMap(pMapId);
            GameMap.OnEnterMap(pMapId);

            //通知所有玩家进入地图
            EnterMapS2c enterMsg = new EnterMapS2c();
            enterMsg.mapId = CurrMapId;
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.EnterMapS2c, enterMsg);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="pGameLevelId"></param>
        public void StartGame(int pGameLevelId)
        {
            if (!IAConfig.Config.GameLevelCfg.ContainsKey(pGameLevelId))
            {
                Debug.LogError("开始游戏失败，没有对应的关卡:" + pGameLevelId);
                return;
            }

            //设置数据
            GameLevelCfg cfg = IAConfig.Config.GameLevelCfg[pGameLevelId];
            CurrGameLevel = pGameLevelId;
            TimeScale = 1.0f;
            GameTime = cfg.time;
            GameDeltaTime = NetworkLogicTimer.FixedDelta;

            GameMode.OnStartGame(pGameLevelId);
            GameData.OnStartGame(pGameLevelId);
            GameMap.OnStartGame(pGameLevelId);

            //通知所有玩家开始游戏
            StartGameS2c msg = new StartGameS2c();
            msg.RetCode = 0;
            msg.gameCfgId = pGameLevelId;
            msg.gameMode = (int)ModeType;
            List<ServerGamerData> gamers = NetServerLocate.GameCtrl.GameData.Gamers;
            foreach (ServerGamerData gamer in gamers)
            {
                msg.Gamers.Add(gamer.CollectGamerInfo());
            }
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.StartGameS2c, msg);

            State = GameState.Start;
        }

        public void PauseGame()
        {
            TimeScale = 0;
        }

        public void ReuseGame()
        {
            TimeScale = 1;
        }

        public void EndGame()
        {
            State = GameState.End;

            ResetTime();

            GameMode.OnEndGame();
            GameData.OnEndGame();
            GameMap.OnEndGame();

            GameEndS2c msg = new GameEndS2c();
            List<ServerGamerData> gamers = NetServerLocate.GameCtrl.GameData.Gamers;
            foreach (ServerGamerData gamer in gamers)
            {
                msg.Gamers.Add(gamer.CollectGamerInfo());
            }
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.GameEndS2c, msg);
        }

        #endregion

        #region 网络消息

        public void OnJoinRoom(NetPeer pPeer, JoinRoomC2s pMsgData)
        {
            ServerGamerData joinGamer = GameData.AddGamer(pPeer, pMsgData.Gamer);

            //通知加入成功
            JoinRoomS2c msg = new JoinRoomS2c();
            msg.RetCode = 0;
            msg.gameMode = (int)ModeType;
            msg.roomMastergamerUid = GameMode.Mode.RoomMasterUid;
            msg.selfgamerUid = joinGamer.GamerUid;
            NetServerLocate.Net.SendTo(pPeer, (ushort)RoomMsgDefine.JoinRoomS2c, msg);

            //广播其他人有人加入
            RoomMembersChangeS2c chanegMsg = new RoomMembersChangeS2c();
            foreach (ServerGamerData item in GameData.Gamers)
            {
                chanegMsg.Gamers.Add(item.CollectGamerInfo());
            }
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.RoomMembersChangeS2c, chanegMsg);
        }

        public void OnGamerInput(NetPeer pPeer, GamerInputC2s pMsgData)
        {
            ServerGamerData gamer = GameData.GetGamer(pMsgData.gamerUid);
            if (gamer == null) 
            {
                return;
            }

            gamer.OnRecInputMsg(pMsgData);
        }

        #endregion
    }
}
