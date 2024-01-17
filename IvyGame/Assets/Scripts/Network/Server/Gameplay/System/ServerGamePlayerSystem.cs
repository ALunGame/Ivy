using IAEngine;
using Proto;
using System.Collections.Generic;

namespace Game.Network.Server
{
    internal class ServerGamePlayerSystem : ServerGameSystem
    {
        private float syncMoveTimer;
        private float syncMoveDelTime = 0.2f;
        private PlayerMoveS2c moveMsg = new PlayerMoveS2c();

        protected override void OnInit()
        {
            syncMoveTimer = 0;
        }

        protected override void OnUpdate(float pDeltaTime, float pRealElapseSeconds)
        {
            if (pRealElapseSeconds > syncMoveTimer)
            {
                SyncPlayerMovePos(pDeltaTime);
                syncMoveTimer = pRealElapseSeconds + syncMoveDelTime;
            }
        }

        private void SyncPlayerMovePos(float pDeltaTime)
        {
            List<ServerPlayer> players = NetServerLocate.Game.Room.Getplayers();
            if (players.IsLegal())
            {
                foreach (ServerPlayer player in players)
                {
                    float moveDel = player.Speed * pDeltaTime;
                    int moveDir = player.MoveDir.GetValue();
                    player.MoveDelCache += moveDel * moveDir;

                    //广播消息
                    moveMsg.RetCode = 0;
                    moveMsg.playerUid = player.Uid;
                    moveMsg.movePos = new NetVector2()
                    {
                        X = NetworkGeneral.EncodeMoveMsgValue(player.MoveDir.HorDir, moveDel),
                        Y = NetworkGeneral.EncodeMoveMsgValue(player.MoveDir.VerDir, moveDel),
                    };
                    NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerMoveS2c, moveMsg);
                }
            }
        }
    }
}
