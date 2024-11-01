﻿//using IAEngine;
//using Proto;
//using System.Collections.Generic;

//namespace Game.Network.Server
//{
//    internal class ServerGamePlayerSystem : ServerGameSystem
//    {
//        private float syncMoveTimer;
//        private float syncMoveDelTime = 0.2f;
//        private PlayerMoveS2c moveMsg = new PlayerMoveS2c();

//        protected override void OnInit()
//        {
//            syncMoveTimer = 0;
//        }

//        protected override void OnUpdate(float pDeltaTime, float pRealElapseSeconds)
//        {
//            if (pRealElapseSeconds >= syncMoveTimer)
//            {
//                SyncPlayerMovePos(pDeltaTime);
//                syncMoveTimer = pRealElapseSeconds + syncMoveDelTime;
//            }
//        }

//        private void SyncPlayerMovePos(float pDeltaTime)
//        {
//            List<ServerPlayer> players = NetServerLocate.Game.Room.GetPlayers();
//            if (players.IsLegal())
//            {
//                foreach (ServerPlayer player in players)
//                {
//                    float moveDel = player.Speed * pDeltaTime;
//                    if (moveDel <= 0)
//                    {
//                        continue;
//                    }

//                    ServerPos movePos = player.MoveDir.CalcMovePos(player.Pos,moveDel);

//                    byte checkX = movePos.x < 0 ? (byte)0 : (byte)(movePos.x);
//                    if (NetServerLocate.Game.Room.Map.CheckPosXInBorder(player.GridPos.x) && NetServerLocate.Game.Room.Map.CheckPosXInBorder(checkX))
//                        movePos.x = player.Pos.x;

//                    byte checkY = movePos.y < 0 ? (byte)0 : (byte)(movePos.y);
//                    if (NetServerLocate.Game.Room.Map.CheckPosYInBorder(player.GridPos.y) && NetServerLocate.Game.Room.Map.CheckPosYInBorder(checkY))
//                        movePos.y = player.Pos.y;

//                    player.SetPos(movePos.x,movePos.y);

//                    moveMsg.playerUid = player.Uid;

//                    //检测边界
//                    byte nextPosX = player.ToGridPos(movePos.x);
//                    byte nextPosY = player.ToGridPos(movePos.y);
//                    if (!NetServerLocate.Game.Room.Map.CheckPointIsLegal(nextPosX, nextPosY))
//                    {
//                        player.UpdateSpeed(-player.Speed);
//                        //广播消息
//                        moveMsg.movePos = new NetVector2()
//                        {
//                            X = NetworkGeneral.EncodeMoveMsgValue(player.GridPos.x),
//                            Y = NetworkGeneral.EncodeMoveMsgValue(player.GridPos.y),
//                        };
//                        NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerMoveS2c, moveMsg);
//                        continue;
//                    }
//                    else
//                    {
//                        moveMsg.movePos = new NetVector2()
//                        {
//                            X = NetworkGeneral.EncodeMoveMsgValue(player.Pos.x),
//                            Y = NetworkGeneral.EncodeMoveMsgValue(player.Pos.y),
//                        };
//                        NetServerLocate.Net.Broadcast((ushort)RoomMsgDefine.PlayerMoveS2c, moveMsg);

//                        //判断移动到下一个格子
//                        NetServerLocate.Game.Room.TestPlayerMove(player.Uid, nextPosX, nextPosY);
//                    }
//                }
//            }
//        }
//    }
//}
