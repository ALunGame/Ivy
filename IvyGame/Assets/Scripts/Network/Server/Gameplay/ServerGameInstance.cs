using Gameplay;
using IAEngine;
using Proto;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network.Server
{
    internal enum GameState
    {
        Wait,
        Start,
        End,
    }

    internal class ServerGameInstance : MonoBehaviour
    {
        public int GameCfgId {  get; private set; }
        public GameModeType GameMode {  get; private set; }
        public ServerGameRoom Room { get; private set; }

        /// <summary>
        /// 当前游戏时间
        /// </summary>
        public GameState CurrGameState { get; private set; }

        /// <summary>
        /// 当前游戏时间
        /// </summary>
        public float CurrGameTime { get; private set; }

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

        //玩法系统
        private List<ServerGameSystem> systems = new List<ServerGameSystem>()
        {
            new ServerGameRuleSystem(),
            new ServerGamePlayerSystem(),
        };

        #region 生命周期

        /// <summary>
        /// 创建一局游戏
        /// </summary>
        /// <param name="cfgId"></param>
        /// <param name="modeType"></param>
        public void Create(int cfgId, GameModeType modeType)
        {
            CurrGameState = GameState.Wait;

            GameCfgId = cfgId;
            GameMode = modeType;

            Room = new ServerGameRoom();
            Room.Create((byte)TempConfig.MapSize.x, (byte)TempConfig.MapSize.y, 10);

            Room.Map.Evt_PointCampChange += OnCampChange;
            Room.Map.Evt_KillPlayer += OnKillPlayer;

            Room.Map.Evt_AddPlayerPathPoint += OnAddPlayerPathPoint;
            Room.Map.Evt_RemovePlayerPathPoint += OnRemovePlayerPathPoint;
            Room.Map.Evt_RemovePlayerPath += OnRemovePlayerPath;

            foreach (ServerGameSystem system in systems)
                system.Init();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            CurrGameTime = 0;
            CurrGameState = GameState.Start;

            //创建玩家
            List<ServerToken> serverTokens = NetServerLocate.TokenCenter.GetServerTokens();
            for (int i = 0; i < serverTokens.Count; i++)
            {
                ServerToken token = serverTokens[i];
                PlayerInfo playerInfo = token.CollectPlayerInfo();
                AddServerPlayerInfo addInfo = new AddServerPlayerInfo(playerInfo.Uid, playerInfo.Id, playerInfo.Name, (byte)playerInfo.Camp);
                AddPlayer(addInfo);
            }

            //系统开始游戏
            foreach (ServerGameSystem system in systems)
                system.StartGame();

            //地图添加玩家
            List<ServerPlayer> players = Room.GetPlayers();
            foreach (ServerPlayer player in players)
            {
                Room.Map.AddPlayer(player);
            }

            //广播开始游戏
            StartGameS2c msg = new StartGameS2c();
            msg.RetCode = 0;
            msg.gameCfgId = GameCfgId;
            msg.gameMode = (int)GameMode;
            foreach (ServerToken token in serverTokens)
            {
                msg.Players.Add(token.CollectPlayerInfo());
            }
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.StartGameS2c, msg);

            //系统开始游戏后
            foreach (ServerGameSystem system in systems)
                system.AfterStartGame();
        }

        private void Awake()
        {
            UpdateRealElapseSeconds = 0;
            UpdateDeltaTime = Time.deltaTime;
            UpdateTimeScale = Time.timeScale;
        }

        /// <summary>
        /// 每帧逻辑更新
        /// </summary>
        public void Update()
        {
            UpdateRealElapseSeconds += UpdateDeltaTime * UpdateTimeScale;

            CurrGameTime += UpdateDeltaTime * UpdateTimeScale;
            if (CurrGameState == GameState.Start && CurrGameTime >= TempConfig.GameTotalTime)
            {
                EndGame();
            }

            foreach (ServerGameSystem system in systems)
                system.Update(UpdateDeltaTime * UpdateTimeScale, UpdateRealElapseSeconds);
        }

        private void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            CurrGameState = GameState.End;
            foreach (ServerGameSystem system in systems)
                system.EndGame();
        }

        public void Clear()
        {
            foreach (ServerGameSystem system in systems)
                system.Clear();
        }

        #endregion

        #region 玩家

        /// <summary>
        /// 添加玩家
        /// </summary>
        public void AddPlayer(AddServerPlayerInfo addInfo)
        {
            if (Room.GetPlayer(addInfo.uid) != null)
                return;
            Room.AddPlayer(new ServerPlayer(addInfo));
        }

        /// <summary>
        /// 复活玩家
        /// </summary>
        public void RebornPlayer(int diePlayerUid)
        {
            ServerPlayer diePlayer = Room.GetPlayer(diePlayerUid);
            if (diePlayer == null)
                return;

            PlayerRebornS2c rebornMsg = new PlayerRebornS2c();
            rebornMsg.playerUid = diePlayerUid;
            rebornMsg.RetCode = 0;

            //找到占领区域
            ServerPoint point = Room.Map.GetRandomPointInCamp(diePlayer.Camp);
            if (point != null)
            {
                diePlayer.SetGridPos(point.x, point.y);
                diePlayer.SetPos(point.x, point.y);
                diePlayer.Reborn();

                rebornMsg.Pos = new NetVector2()
                {
                    X = point.x,
                    Y = point.y
                };
            }
            else
            {
                ServerRect rect = Room.Map.CreateCampRect(5, 5);
                if (rect != null)
                {
                    Room.Map.ChangRectCamp(rect, diePlayer.Camp);

                    ServerPoint randomPoint = rect.TakeRandomPoint();
                    diePlayer.SetGridPos(randomPoint.x, randomPoint.y);
                    diePlayer.SetPos(randomPoint.x, randomPoint.y);
                    diePlayer.Reborn();

                    rebornMsg.Pos = new NetVector2()
                    {
                        X = randomPoint.x,
                        Y = randomPoint.y
                    };
                }
                else
                {
                    rebornMsg.RetCode = 1;
                    //TODO:
                    NetServerLocate.Log.LogError("没有复活位置了》》》》》", diePlayer.Uid);
                }
            }

            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerRebornS2c, rebornMsg);
        }

        /// <summary>
        /// 击杀玩家
        /// </summary>
        private void KillPlayer(int diePlayerUid, int killerPlayerUid)
        {
            ServerPlayer diePlayer = Room.GetPlayer(diePlayerUid);
            if (diePlayer != null)
            {
                diePlayer.DieCnt++;
                diePlayer.Die();
            }

            ServerPlayer killerPlayer = Room.GetPlayer(killerPlayerUid);
            if (killerPlayer != null)
            {
                killerPlayer.KillCnt++;
            }
        } 

        #endregion

        #region RoomEvent

        private CampAreaChangeS2c campAreaChangeMsg = new CampAreaChangeS2c();
        private void OnCampChange(byte posX, byte posY, byte camp)
        {
            campAreaChangeMsg.Camp = camp;
            campAreaChangeMsg.Pos = new NetVector2() { X = posX, Y = posY };
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.CampAreaChangeS2c, campAreaChangeMsg);
        }

        private PlayerDieS2c playerDieMsg = new PlayerDieS2c();
        private void OnKillPlayer(int diePlayerUid, int killPlayerUid)
        {
            KillPlayer(diePlayerUid, killPlayerUid);

            playerDieMsg.diePlayerUid = diePlayerUid;
            playerDieMsg.rebornTime = TempConfig.RebornTime;
            playerDieMsg.killerPlayerUid = killPlayerUid;
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerDieS2c, playerDieMsg);
        }

        private PlayerPathChangeS2c playerPathChangeMsg = new PlayerPathChangeS2c();
        private void OnAddPlayerPathPoint(int playerUid, byte posX, byte posY)
        {
            playerPathChangeMsg.playerUid = playerUid;
            playerPathChangeMsg.Operate = 1;
            playerPathChangeMsg.Pos = new NetVector2() { X = posX, Y = posY};
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerPathChangeS2c, playerPathChangeMsg);
        }

        private void OnRemovePlayerPathPoint(int playerUid, byte posX, byte posY)
        {
            playerPathChangeMsg.playerUid = playerUid;
            playerPathChangeMsg.Operate = 2;
            playerPathChangeMsg.Pos = new NetVector2() { X = posX, Y = posY };
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerPathChangeS2c, playerPathChangeMsg);
        }

        private void OnRemovePlayerPath(int playerUid)
        {
            playerPathChangeMsg.playerUid = playerUid;
            playerPathChangeMsg.Operate = 3;
            playerPathChangeMsg.Pos = new NetVector2();
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerPathChangeS2c, playerPathChangeMsg);
        }

        #endregion

        #region PlayerEvent

        public void OnPlayerMove()
        {

        }

        #endregion
    }
}
