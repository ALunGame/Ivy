using Game.Network.Client;
using Gameplay;
using Gameplay.GameData;
using IAUI;
using Proto;
using ProtoBuf;
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

        private UINetworkMsgGlue OnJoinRoomS2c;
        private UINetworkMsgGlue OnLeaveRoomS2c;
        private UINetworkMsgGlue OnRoomMembersChangeS2c;

        public override void OnAwake()
        {
            BtnUtil.SetClick(transform, "Center/CloseBtn", () =>
            {
                Close();
            });

            BtnUtil.SetClick(transform, "Center/StartBtn", () =>
            {
                if (!GameplayCtrl.Instance.GameData.CheckIsRoomMaster())
                {
                    return;
                }

                GameModeType modeType = GameplayCtrl.Instance.GameModeType;
                Logger.Client?.LogWarning("发送开始游戏》》》》》", modeType);

                StartGameC2s msg = new StartGameC2s();
                msg.gameCfgId = 1;
                msg.gameMode = (int)modeType;
                NetClientLocate.Net.Send((ushort)RoomMsgDefine.StartGameC2s, msg);
            });
            OnJoinRoomS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.JoinRoomS2c, (msgBody) =>
            {
                Refresh();
            });

            OnLeaveRoomS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.LeaveRoomS2c, (msgBody) =>
            {
                Refresh();
            });

            OnRoomMembersChangeS2c = new UINetworkMsgGlue(this, (ushort)RoomMsgDefine.RoomMembersChangeS2c, (msgBody) =>
            {
                Refresh();
            });
        }

        public override void OnShow()
        {
            Refresh();
            transform.Find("Center/StartBtn").gameObject.SetActive(GameplayCtrl.Instance.GameData.CheckIsRoomMaster());
        }

        public override void OnHide()
        {
        }

        private void Refresh()
        {
            Refresh_Info();
            GameModeType modeType = GameplayCtrl.Instance.GameModeType;
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
            GameModeType modeType = GameplayCtrl.Instance.GameModeType;
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
            List<GamerData> gamers = GameplayCtrl.Instance.GameData.Gamers.Gamers;
            for (int i = 0; i < gamers.Count; i++)
            {
                GamerData gamer = gamers[i];
                GameObject gamerGo = singlePlayerInfoCache.Take();
                Refresh_PlayerInfo(gamerGo, gamer);
            }
        }

        private void Refresh_Team()
        {

        }

        private void Refresh_PlayerInfo(GameObject playerGo, GamerData gamerData)
        {

        }
    }
}
