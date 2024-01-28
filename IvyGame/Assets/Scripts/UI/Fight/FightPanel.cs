using Game.Network.Client;
using Gameplay.Player;
using Gameplay;
using IAUI;
using UnityEngine;
using UnityEngine.UI;
using Proto;

namespace Game.UI
{
    internal class FightPanel_Model : UIModel
    {

    }

    internal class FightPanel : UIPanel<FightPanel_Model>
    {
        private UIComGlue<RectTransform> gameInfoTrans = new UIComGlue<RectTransform>("Top/GameInfo");
        private UIComGlue<RectTransform> moveBoxTrans = new UIComGlue<RectTransform>("Left/MoveBox");

        private UIComGlue<Text> killInfo = new UIComGlue<Text>("Top/GameInfo/KillInfo/Num");
        private UIComGlue<Text> gameTimeInfo = new UIComGlue<Text>("Top/GameInfo/GameTime/Num");

        private UIUpdateGlue updateGlue = new UIUpdateGlue();
        private GamePlayer localPlayer;

        private Vector2Int moveDir = new Vector2Int();
        private PlayerMoveC2s moveMsg = new PlayerMoveC2s();

        public override void OnAwake()
        {
            updateGlue.SetFunc(Update);
            localPlayer = GameplayLocate.GameIns.GetPlayer(NetClientLocate.LocalToken.PlayerUid);

            moveMsg.playerUid = localPlayer.Uid;
            moveMsg.moveDir = new NetVector2();

            BtnUtil.SetClick(transform, "Left/MoveBox/UpBtn", () =>
            {
                SendMoveMsg(0, 1);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/DownBtn", () =>
            {
                SendMoveMsg(0, -1);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/LeftBtn", () =>
            {
                SendMoveMsg(-1, 0);
            });

            BtnUtil.SetClick(transform, "Left/MoveBox/RightBtn", () =>
            {
                SendMoveMsg(1, 0);
            });
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            killInfo.Com.text = $"{localPlayer.KillCnt}/{localPlayer.DieCnt}";
            gameTimeInfo.Com.text = $"{(int)GameplayLocate.GameIns.CurrGameTime}s";
        }

        private void SendMoveMsg(int x,int y)
        {
            moveMsg.moveDir.X = x;
            moveMsg.moveDir.Y = y;
            NetClientLocate.Net.Send((ushort)RoomMsgDefine.PlayerMoveC2s, moveMsg);
        }
    }
}
