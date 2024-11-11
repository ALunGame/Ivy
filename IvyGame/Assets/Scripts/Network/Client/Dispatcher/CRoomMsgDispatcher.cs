using Game.Network.Client;
using Gameplay;
using Gameplay.GameData;
using IAEngine;
using Proto;
using UnityEngine;

namespace Game.Network.CDispatcher
{
    internal class CRoomMsgDispatcher : NetClientDispatcher
    {
        internal CRoomMsgDispatcher(NetClientDispatcherMapping InMapping) : base(InMapping)
        {
            AddDispatch<CreateRoomS2c>((ushort)RoomMsgDefine.CreateRoomS2c,OnCreateRoomS2c);
            AddDispatch<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c,OnJoinRoomS2c);
            AddDispatch<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c,OnLeaveRoomS2c);
            AddDispatch<RoomMembersChangeS2c>((ushort)RoomMsgDefine.RoomMembersChangeS2c,OnRoomMembersChangeS2c);
            AddDispatch<StartGameS2c>((ushort)RoomMsgDefine.StartGameS2c,OnStartGameS2c);
            AddDispatch<EnterMapS2c>((ushort)RoomMsgDefine.EnterMapS2c,OnEnterMapS2c);
            AddDispatch<ServerStateS2c>((ushort)RoomMsgDefine.ServerStateS2c,OnServerStateS2c);
            AddDispatch<GamerInputS2c>((ushort)RoomMsgDefine.GamerInputS2c,OnGamerInputS2c);
            AddDispatch<GamerDieS2c>((ushort)RoomMsgDefine.GamerDieS2c,OnGamerDieS2c);
            AddDispatch<GamerRebornS2c>((ushort)RoomMsgDefine.GamerRebornS2c,OnGamerRebornS2c);
            AddDispatch<GamerPathChangeS2c>((ushort)RoomMsgDefine.GamerPathChangeS2c,OnGamerPathChangeS2c);
            AddDispatch<ChangeGridCampS2c>((ushort)RoomMsgDefine.ChangeGridCampS2c,OnChangeGridCampS2c);
            AddDispatch<GameEndS2c>((ushort)RoomMsgDefine.GameEndS2c,OnGameEndS2c);

        }
        
        
        private void OnCreateRoomS2c(CreateRoomS2c MsgData)
        {
        }

        private void OnJoinRoomS2c(JoinRoomS2c MsgData)
        {
            GameplayGlobal.Data.RoomMasterUid = MsgData.roomMastergamerUid;
            GameplayGlobal.Data.SelfGamerUid = MsgData.selfgamerUid;

            NetClientLocate.LocalToken.SetLocalPlayerUid(MsgData.selfgamerUid);           
        }

        private void OnLeaveRoomS2c(LeaveRoomS2c MsgData)
        {
            GameplayCtrl.Instance.GameData.Gamers.RemoveGamer(MsgData.gamerUid);
        }

        private void OnRoomMembersChangeS2c(RoomMembersChangeS2c MsgData)
        {
            if (MsgData.Gamers.IsLegal())
            {
                for (int i = 0; i < MsgData.Gamers.Count; i++)
                {
                    GameplayCtrl.Instance.GameData.Gamers.UpdateGamer(MsgData.Gamers[i]);
                }
            }
        }

        private void OnStartGameS2c(StartGameS2c MsgData)
        {
            if (MsgData.Gamers.IsLegal())
            {
                for (int i = 0; i < MsgData.Gamers.Count; i++)
                {
                    GameplayCtrl.Instance.GameData.Gamers.UpdateGamer(MsgData.Gamers[i]);
                }
            }

            GameplayCtrl.Instance.StartGame(MsgData.gameCfgId);


        }

        private void OnEnterMapS2c(EnterMapS2c MsgData)
        {
            GameplayCtrl.Instance.EnterMap(MsgData.mapId);
        }

        private void OnServerStateS2c(ServerStateS2c MsgData)
        {
            GameplayCtrl.Instance.OnReceiveServerState(MsgData);
        }

        private void OnGamerInputS2c(GamerInputS2c MsgData)
        {
            
        }

        private void OnGamerDieS2c(GamerDieS2c MsgData)
        {
            if (MsgData.dieGamerInfos.IsLegal())
            {
                for (int i = 0; i < MsgData.dieGamerInfos.Count; i++)
                {
                    GamerDieInfo info = MsgData.dieGamerInfos[i];

                    GamerData dieGamerData = GameplayGlobal.Data.Gamers.GetGamer(info.gamerUid);
                    dieGamerData?.Die(info);

                    GamerData killGamerData = GameplayGlobal.Data.Gamers.GetGamer(info.killergamerUid);
                    killGamerData?.Kill(info);
                }
            }
        }

        private void OnGamerRebornS2c(GamerRebornS2c MsgData)
        {
            GamerData gamerData = GameplayGlobal.Data.Gamers.GetGamer(MsgData.gamerUid);
            gamerData?.Reborn(MsgData);
        }

        private void OnGamerPathChangeS2c(GamerPathChangeS2c MsgData)
        {
            GamerData gamerData = GameplayGlobal.Data.Gamers.GetGamer(MsgData.gamerUid);
            //Add
            if (MsgData.Operate == 1)
            {
                gamerData.AppPathPoint(MsgData.Pos.ToVector2Int());
            }
            //Remove
            else if (MsgData.Operate == 2)
            {
                gamerData.RemovePathPoint(MsgData.Pos.ToVector2Int());
            }
            //Clear
            else if (MsgData.Operate == 3)
            {
                gamerData.ClearPath();
            }
        }

        private void OnChangeGridCampS2c(ChangeGridCampS2c MsgData)
        {
            foreach (var gridPos in MsgData.gridPosLists)
            {
                GameMapGridData gridData = GameplayGlobal.Data.Map.GetGridData(new Vector2Int((int)gridPos.X, (int)gridPos.Y));
                gridData.Camp.Value = MsgData.Camp;
            }
        }

        private void OnGameEndS2c(GameEndS2c MsgData)
        {
            GameplayGlobal.Ctrl.EndGame();
        }


    }
}

