using Game.Network.Client;
using Gameplay;
using IAUI;
using Proto;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    internal class RoomPanel_Model : UIModel
    {
    }

    internal class RoomPanel : UIPanel<RoomPanel_Model>
    {
        private UIComGlue<RectTransform> roomBox = new UIComGlue<RectTransform>("Center/RoomBox");
        private UIComGlue<RectTransform> roomInfo = new UIComGlue<RectTransform>("Center/RoomBox/RoomInfo");
        private UIComGlue<RectTransform> roomPlayers = new UIComGlue<RectTransform>("Center/RoomBox/Players");

        private UICacheGlue singlePlayerInfoCache = new UICacheGlue("Center/Prefab/PlayerInfo", "Center/RoomBox/Players/Single/List", true);
        private UICacheGlue teamPlayerInfoCache = new UICacheGlue("Center/Prefab/PlayerInfo", "Center/RoomBox/Players/Team/List", true);

        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "Center/CloseBtn", () =>
            {
                Close();
            });

            BtnUtil.SetClick(transform, "Center/StartBtn", () =>
            {
                if (!GameplayLocate.UserData.Room.CheckIsRoomMaster())
                {
                    return;
                }

                GameModeType modeType = GameplayLocate.UserData.Room.GameMode;
                NetClientLocate.Log.LogWarning("发送开始游戏》》》》》", modeType);

                StartGameC2s msg = new StartGameC2s();
                msg.gameCfgId = 1;
                msg.gameMode = (int)modeType;
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.StartGameC2s, msg);
            });
        }

        public override void OnShow()
        {
            Refresh();
            transform.Find("Center/StartBtn").gameObject.SetActive(GameplayLocate.UserData.Room.CheckIsRoomMaster());

            NetClientLocate.LocalToken.AddListen<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c, OnJoinRoomS2c);
            NetClientLocate.LocalToken.AddListen<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c, OnLeaveRoomS2c);
            NetClientLocate.LocalToken.AddListen<RoomMembersChangeS2c>((ushort)RoomMsgDefine.RoomMembersChangeS2c, OnRoomMembersChangeS2c);
        }

        public override void OnHide()
        {
            NetClientLocate.LocalToken.RemoveListen<JoinRoomS2c>((ushort)RoomMsgDefine.JoinRoomS2c, OnJoinRoomS2c);
            NetClientLocate.LocalToken.AddListen<LeaveRoomS2c>((ushort)RoomMsgDefine.LeaveRoomS2c, OnLeaveRoomS2c);
            NetClientLocate.LocalToken.RemoveListen<RoomMembersChangeS2c>((ushort)RoomMsgDefine.RoomMembersChangeS2c, OnRoomMembersChangeS2c);
        }

        private void Refresh()
        {
            Refresh_Info();
            GameModeType modeType = GameplayLocate.UserData.Room.GameMode;
            if (modeType == GameModeType.Local)
            {
                Refresh_Local();
            }
            else if (modeType == GameModeType.Single)
            {
                Refresh_Single();
            }
            else if (modeType == GameModeType.Team)
            {
                Refresh_Team();
            }
        }

        private void Refresh_Info()
        {
            GameModeType modeType = GameplayLocate.UserData.Room.GameMode;
            string modeStr = "单人";
            if (modeType == GameModeType.Team)
                modeStr = "团队";
            else if (modeType == GameModeType.Single)
                modeStr = "单人";
            TextUtil.SetText(roomInfo.Com, "GameMode/Str", modeStr);
        }

        private void Refresh_Local()
        {

        }

        private void Refresh_Single()
        {
            singlePlayerInfoCache.RecycleAll();
            List<PlayerInfo> players = GameplayLocate.UserData.Room.PlayerInfos;
            for (int i = 0; i < players.Count; i++)
            {
                PlayerInfo player = players[i];
                GameObject playerGo = singlePlayerInfoCache.Take();
                Refresh_PlayerInfo(playerGo, player);
            }
        }

        private void Refresh_Team()
        {

        }

        private void Refresh_PlayerInfo(GameObject playerGo, PlayerInfo info)
        {

        }

        #region 网络消息

        private void OnJoinRoomS2c(JoinRoomS2c c)
        {
            Refresh();
        }

        private void OnLeaveRoomS2c(LeaveRoomS2c c)
        {
            Refresh();
        }


        private void OnRoomMembersChangeS2c(RoomMembersChangeS2c c)
        {
            Refresh();
        }


        #endregion
    }
}
