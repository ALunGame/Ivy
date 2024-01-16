using Gameplay;
using IAEngine;
using Proto;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class ServerGame
    {
        public int GameCfgId {  get; private set; }
        public GameModeType GameMode {  get; private set; }
        public ServerGameRoom Room { get; private set; }

        /// <summary>
        /// 创建一局游戏
        /// </summary>
        /// <param name="cfgId"></param>
        /// <param name="modeType"></param>
        public void Create(int cfgId, GameModeType modeType)
        {
            Room = new ServerGameRoom();
            Room.Create(30, 30, 10);

            Room.Map.Evt_PointCampChange += OnCampChange;
            Room.Map.Evt_KillPlayer += OnKillPlayer;

            Room.Map.Evt_AddPlayerPathPoint += OnAddPlayerPathPoint;
            Room.Map.Evt_RemovePlayerPathPoint += OnRemovePlayerPathPoint;
            Room.Map.Evt_RemovePlayerPath += OnRemovePlayerPath;
        }

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
        private void RebornPlayer(int diePlayerUid)
        {
            ServerPlayer diePlayer = Room.GetPlayer(diePlayerUid);
            if (diePlayer == null)
                return;

            //找到占领区域
            ServerPoint point = Room.Map.GetRandomPointInCamp(diePlayer.Camp);
            if (point != null)
            {
                diePlayer.SetPos(point.x,point.y);
                diePlayer.Reborn();
            }
            else
            {
                List<ServerRect> rects = Room.Map.CreateCampRect(5, 5, 1);
                if (rects.IsLegal())
                {
                    ServerRect rect = rects[0];
                    Room.Map.ChangRectCamp(rect, diePlayer.Camp);
                    diePlayer.SetPos(rect.min.x, rect.min.y);
                    diePlayer.Reborn();
                }
                else
                {
                    NetServerLocate.Log.LogError("没有复活位置了》》》》》", diePlayer.Uid);
                }
            }
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
            }

            ServerPlayer killerPlayer = Room.GetPlayer(killerPlayerUid);
            if (killerPlayer != null)
            {
                killerPlayer.KillCnt++;
            }
        }

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
            RebornPlayer(diePlayerUid);

            playerDieMsg.diePlayerUid = diePlayerUid;
            playerDieMsg.rebornTime = 0;
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
