using Gameplay;
using LiteNetLib;
using Proto;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class ServerGameModeCtrl : ServerGameplayProcess
    {
        /// <summary>
        /// 游戏模式
        /// </summary>
        public GameModeType ModeType { get; private set; }

        /// <summary>
        /// 游戏模式数据
        /// </summary>
        public BaseServerGameMode Mode { get; private set; }

        public override void OnCreateGame(GameModeType pModeType, NetPeer pRoomMasterPeer, string pRoomMasterUid, CreateRoomC2s pMsgData)
        {
            ModeType = pModeType;
            if (ModeType == GameModeType.Single)
            {
                Mode = new ServerGameMode_Single();
            }
            else
            {
                NetServerLocate.Log.LogError("OnCreateGame失败！！该游戏模式尚未支持", ModeType);
                return;
            }

            Mode.Init(pRoomMasterUid, pMsgData);
        }

        public override void OnEnterMap(int pMapId)
        {
            Mode.EnterMap(pMapId);
        }

        public override void OnStartGame(int pGameLevelId)
        {
            Mode.StartGame(pGameLevelId);
        }

        #region 游戏规则

        public virtual void GamerDie(List<string> pDieGamerUids, string pKillGamerUid)
        {
            ServerGamerData killGamerData = NetServerLocate.GameCtrl.GameData.GetGamer(pKillGamerUid);

            GamerDieS2c msg = new GamerDieS2c();

            for (int i = 0; i < pDieGamerUids.Count; i++)
            {
                string gamerUid = pDieGamerUids[i];
                ServerGamerData gamerData = NetServerLocate.GameCtrl.GameData.GetGamer(gamerUid);
                gamerData.Die();
                killGamerData.Kill();

                Mode.OnGamerDie(gamerData);

                GamerDieInfo gamerDieInfo = new GamerDieInfo();
                gamerDieInfo.gamerUid = gamerUid;
                gamerDieInfo.rebornTime = (int)gamerData.RebornTime.Value;
                gamerDieInfo.killergamerUid = pKillGamerUid;

                msg.dieGamerInfos.Add(gamerDieInfo);
            }

            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.GamerDieS2c, msg);
        }


        public virtual void GamerReborn(string pGamerUid)
        {
            ServerGamerData gamerData = NetServerLocate.GameCtrl.GameData.GetGamer(pGamerUid);
            gamerData.Reborn();

            Mode.OnGamerReborn(gamerData);

            GamerRebornS2c msg = new GamerRebornS2c();
            msg.gamerUid = pGamerUid;
            msg.Pos = gamerData.Position.ToNetVector2();
            NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.GamerRebornS2c, msg);
        }

        #endregion
    }
}
